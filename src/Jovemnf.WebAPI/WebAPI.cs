using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Jovemnf.WebAPI.Internal;

namespace Jovemnf.WebAPI
{
    public class WebAPI : IDisposable
    {
        // Re-expose MethodRequest for backward compatibility in WebAPI class
        public enum MethodRequest
        {
            POST, GET, DELETE, PUT, PATCH
        }

        private readonly WebAPIEngine _engine = new WebAPIEngine();
        private int _timeoutSeconds = 10;
        private Jovemnf.WebAPI.MethodRequest _method = Jovemnf.WebAPI.MethodRequest.GET;
        private readonly Uri _uri;
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
        private HttpContent? _content;
        private HttpClient? _instanceClient;

        public WebAPI(Uri uri, int timeout = 10000, MethodRequest metodo = MethodRequest.GET)
        {
            _uri = uri;
            _timeoutSeconds = timeout / 1000;
            _method = (Jovemnf.WebAPI.MethodRequest)metodo;
        }

        // Standard constructor used in tests: (Uri, HttpClient)
        public WebAPI(Uri url, HttpClient client)
            : this(url, 10000, MethodRequest.GET)
        {
            _instanceClient = client;
        }

        public WebAPI(string str, int timeout, MethodRequest metodo) 
            : this(new Uri(str), timeout, metodo) { }

        public WebAPI(string str, MethodRequest metodo, int timeout)
            : this(new Uri(str), timeout, metodo) { }

        public WebAPI(string str, HttpClient client, MethodRequest metodo)
            : this(new Uri(str), 10000, metodo)
        {
            _instanceClient = client;
        }

        public WebAPI(string str, HttpClient client, MethodRequest metodo, int timeout)
            : this(new Uri(str), timeout, metodo)
        {
            _instanceClient = client;
        }

        // Test signature: (Uri, HttpClient, int, MethodRequest)
        public WebAPI(Uri url, HttpClient client, int timeout, MethodRequest method)
            : this(url, timeout, method)
        {
            _instanceClient = client;
        }

        public WebAPI(string str) : this(new Uri(str)) { }
        public WebAPI(Uri uri) : this(uri, 10000, MethodRequest.GET) { }

        public static void SetStaticClient(HttpClient client) => WebAPIEngine.SetClient(client);

        public static Exception CheckException(HttpResponseMessage response) => ResponseValidator.GetException(response);
        public static Exception CheckException(WebException e) => ResponseValidator.GetException(e);

        #region Static API (Axios-like Facade)
        public static Task<T> Get<T>(string url, Dictionary<string, string>? headers = null) => SendStaticAsync<T>(Jovemnf.WebAPI.MethodRequest.GET, url, null, headers);
        public static Task Get(string url, Dictionary<string, string>? headers = null) => SendStaticAsync<string>(Jovemnf.WebAPI.MethodRequest.GET, url, null, headers);
        
        public static Task<T> Post<T>(string url, object data, Dictionary<string, string>? headers = null) => SendStaticAsync<T>(Jovemnf.WebAPI.MethodRequest.POST, url, data, headers);
        public static Task Post(string url, object data, Dictionary<string, string>? headers = null) => SendStaticAsync<string>(Jovemnf.WebAPI.MethodRequest.POST, url, data, headers);

        public static Task<T> Put<T>(string url, object data, Dictionary<string, string>? headers = null) => SendStaticAsync<T>(Jovemnf.WebAPI.MethodRequest.PUT, url, data, headers);
        public static Task<T> Patch<T>(string url, object data, Dictionary<string, string>? headers = null) => SendStaticAsync<T>(Jovemnf.WebAPI.MethodRequest.PATCH, url, data, headers);
        
        public static Task<T> Delete<T>(string url, Dictionary<string, string>? headers = null) => SendStaticAsync<T>(Jovemnf.WebAPI.MethodRequest.DELETE, url, null, headers);

        private static async Task<T> SendStaticAsync<T>(Jovemnf.WebAPI.MethodRequest method, string url, object? data, Dictionary<string, string>? headers)
        {
            using var api = new WebAPI(url);
            if (headers != null) api.SetHeader(headers);
            if (data != null) api.SetJson(data);
            return await api.Send<T>((MethodRequest)method);
        }
        #endregion

        public WebAPI WithHeaders(Dictionary<string, string> headers)
        {
            SetHeader(headers);
            return this;
        }

        /// <summary>
        /// Configura um certificado de cliente (mTLS) para esta instância.
        /// </summary>
        public WebAPI WithCertificate(X509Certificate2 certificate)
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(certificate);
            _instanceClient = new HttpClient(handler);
            return this;
        }

        /// <summary>
        /// Define um HttpClient customizado para esta instância.
        /// </summary>
        public WebAPI WithHttpClient(HttpClient client)
        {
            _instanceClient = client;
            return this;
        }

        /// <summary>
        /// Configura autenticação Basic para esta instância.
        /// </summary>
        public WebAPI WithBasicAuth(string username, string password)
        {
            var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            _headers["Authorization"] = $"Basic {authString}";
            return this;
        }

        public Task<T> Send<T>(MethodRequest method)
        {
            _method = (Jovemnf.WebAPI.MethodRequest)method;
            return Send<T>();
        }

        public Task<T> Send<T>()
        {
            var request = new WebRequestBuilder(_uri)
                .WithMethod((Jovemnf.WebAPI.MethodRequest)_method)
                .WithHeaders(_headers)
                .WithContent(_content)
                .Build();

            return _engine.SendAsync<T>(request, _timeoutSeconds, _instanceClient);
        }

        public Task<dynamic> Send(MethodRequest method)
        {
            _method = (Jovemnf.WebAPI.MethodRequest)method;
            return Send();
        }

        public async Task<dynamic> Send()
        {
            return await Send<object>();
        }

        public Task<string> SendAndGetString(MethodRequest method)
        {
            _method = (Jovemnf.WebAPI.MethodRequest)method;
            return SendAndGetString();
        }

        public async Task<string> SendAndGetString()
        {
            var request = new WebRequestBuilder(_uri)
                .WithMethod((Jovemnf.WebAPI.MethodRequest)_method)
                .WithHeaders(_headers)
                .WithContent(_content)
                .Build();

            return await _engine.SendAndGetStringAsync(request, _timeoutSeconds, _instanceClient);
        }

        public void SetDataPost(Dictionary<string, string> dados)
        {
            _content = new FormUrlEncodedContent(dados);
            if (_method == Jovemnf.WebAPI.MethodRequest.GET) _method = Jovemnf.WebAPI.MethodRequest.POST;
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
            if (_method == Jovemnf.WebAPI.MethodRequest.GET) _method = Jovemnf.WebAPI.MethodRequest.POST;
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

        private string Parse(Dictionary<string, object> dados, string key) => WebUtility.UrlEncode(dados[key]?.ToString() ?? "");
        private string Parse(Dictionary<string, string> dados, string key) => WebUtility.UrlEncode(dados[key] ?? "");
    }
}
