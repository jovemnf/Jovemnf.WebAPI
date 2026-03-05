using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Jovemnf.WebApi.Internal;

namespace Jovemnf.WebApi;

public class Api(Uri uri, int timeout = 10000, MethodRequest metodo = MethodRequest.GET)
    : IDisposable
{

    private readonly WebApiEngine _engine = new ();
    private readonly int _timeoutSeconds = timeout / 1000;
    private MethodRequest _method = metodo;
    private readonly Dictionary<string, string> _headers = new();
    private HttpContent? _content;
    private HttpClient? _instanceClient;

    // Standard constructor used in tests: (Uri, HttpClient)
    public Api(Uri url, HttpClient client)
        : this(url, 10000)
    {
        _instanceClient = client;
    }

    public Api(string str, int timeout, MethodRequest metodo) 
        : this(new Uri(str), timeout, metodo) { }

    public Api(string str, MethodRequest metodo, int timeout)
        : this(new Uri(str), timeout, metodo) { }

    public Api(string str, HttpClient client, MethodRequest metodo)
        : this(new Uri(str), 10000, metodo)
    {
        _instanceClient = client;
    }

    public Api(string str, HttpClient client, MethodRequest metodo, int timeout)
        : this(new Uri(str), timeout, metodo)
    {
        _instanceClient = client;
    }

    // Test signature: (Uri, HttpClient, int, MethodRequest)
    public Api(Uri url, HttpClient client, int timeout, MethodRequest method)
        : this(url, timeout, method)
    {
        _instanceClient = client;
    }

    public Api(string str) : this(new Uri(str)) { }
    public Api(Uri uri) : this(uri, 10000, MethodRequest.GET) { }

    public static void SetStaticClient(HttpClient client) => WebApiEngine.SetClient(client);

    public static Exception CheckException(HttpResponseMessage response) => ResponseValidator.GetException(response);
    public static Exception CheckException(WebException e) => ResponseValidator.GetException(e);

    #region Static API (Axios-like Facade)
    public static Task<WebApiResponse<T>> Get<T>(string url, Dictionary<string, string>? headers = null) => SendStaticAsync<T>(MethodRequest.GET, url, null, headers);
    public static Task<WebApiResponse<dynamic>> Get(string url, Dictionary<string, string>? headers = null) => SendStaticAsync<dynamic>(MethodRequest.GET, url, null, headers);
    public static Task<WebApiResponse<T>> Post<T>(string url, object data, Dictionary<string, string>? headers = null) => SendStaticAsync<T>(MethodRequest.POST, url, data, headers);
    public static Task<WebApiResponse<dynamic>> Post(string url, object data, Dictionary<string, string>? headers = null) => SendStaticAsync<dynamic>(MethodRequest.POST, url, data, headers);
    public static Task<WebApiResponse<T>> Put<T>(string url, object data, Dictionary<string, string>? headers = null) => SendStaticAsync<T>(MethodRequest.PUT, url, data, headers);
    public static Task<WebApiResponse<dynamic>> Put(string url, object data, Dictionary<string, string>? headers = null) => SendStaticAsync<dynamic>(MethodRequest.PUT, url, data, headers);
    public static Task<WebApiResponse<T>> Patch<T>(string url, object data, Dictionary<string, string>? headers = null) => SendStaticAsync<T>(MethodRequest.PATCH, url, data, headers);
    public static Task<WebApiResponse<dynamic>> Patch(string url, object data, Dictionary<string, string>? headers = null) => SendStaticAsync<dynamic>(MethodRequest.PATCH, url, data, headers);
    public static Task<WebApiResponse<T>> Delete<T>(string url, Dictionary<string, string>? headers = null) => SendStaticAsync<T>(MethodRequest.DELETE, url, null, headers);
    public static Task<WebApiResponse<dynamic>> Delete(string url, Dictionary<string, string>? headers = null) => SendStaticAsync<dynamic>(MethodRequest.DELETE, url, null, headers);

    private static async Task<WebApiResponse<T>> SendStaticAsync<T>(MethodRequest method, string url, object? data, Dictionary<string, string>? headers)
    {
        using var api = new Api(url);
        if (headers != null) api.SetHeader(headers);
        if (data != null) api.SetJson(data);
        return await api.Send<T>((MethodRequest)method);
    }
    #endregion

    public Api WithHeaders(Dictionary<string, string> headers)
    {
        SetHeader(headers);
        return this;
    }

    /// <summary>
    /// Configura um certificado de cliente (mTLS) para esta instância.
    /// </summary>
    public Api WithCertificate(X509Certificate2 certificate)
    {
        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(certificate);
        _instanceClient = new HttpClient(handler);
        return this;
    }

    /// <summary>
    /// Define um HttpClient customizado para esta instância.
    /// </summary>
    public Api WithHttpClient(HttpClient client)
    {
        _instanceClient = client;
        return this;
    }

    /// <summary>
    /// Configura autenticação Basic para esta instância.
    /// </summary>
    public Api WithBasicAuth(string username, string password)
    {
        var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        _headers["Authorization"] = $"Basic {authString}";
        return this;
    }

    public Task<WebApiResponse<T>> Send<T>(MethodRequest method)
    {
        _method = method;
        return Send<T>();
    }

    public Task<WebApiResponse<T>> Send<T>()
    {
        var request = new WebRequestBuilder(uri)
            .WithMethod(_method)
            .WithHeaders(_headers)
            .WithContent(_content)
            .Build();

        return WebApiEngine.SendAsync<T>(request, _timeoutSeconds, _instanceClient);
    }

    public Task<dynamic> Send(MethodRequest method)
    {
        _method = method;
        return Send();
    }

    public async Task<dynamic> Send()
    {
        return await Send<object>();
    }

    public void SetDataPost(Dictionary<string, string> dados)
    {
        _content = new FormUrlEncodedContent(dados);
        if (_method == MethodRequest.GET) _method = MethodRequest.POST;
    }

    public void SetDataPost(Dictionary<string, object> dados)
    {
        var dict = new Dictionary<string, string>();
        foreach (var key in dados.Keys) dict.Add(key, dados[key]?.ToString() ?? "");
        SetDataPost(dict);
    }

    public void SetJson(object dados)
    {
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(dados);
        _content = new StringContent(json, Encoding.UTF8, "application/json");
        if (_method == MethodRequest.GET) _method = MethodRequest.POST;
    }

    public void SetHeader(Dictionary<string, string> header)
    {
        foreach (var kvp in header) _headers[kvp.Key] = kvp.Value;
    }

    public void SetHeader(string index, string value) => _headers[index] = value;

    public void Dispose()
    {
        _content?.Dispose();
        // Somente damos Dispose no instanceClient se ele foi criado por nós (ex: WithCertificate)
        // No entanto, para simplicidade e segurança de uso da lib, 
        // frequentemente é melhor o usuário gerenciar o HttpClient se ele o passou.
        // Aqui estamos criando um novo HttpClient no WithCertificate, então deveríamos dispose-lo.
        // Mas o HttpClient injetado via construtor ou WithHttpClient NÃO deve ser disposed pela lib.
        GC.SuppressFinalize(this);
    }
}