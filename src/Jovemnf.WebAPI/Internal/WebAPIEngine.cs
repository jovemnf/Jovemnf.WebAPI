using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Jovemnf.WebApi.Internal;

internal class WebApiEngine
{
    private static HttpClient _staticClient = new ();

    public static void SetClient(HttpClient client) => _staticClient = client;

    public static async Task<WebApiResponse<T>> SendAsync<T>(HttpRequestMessage request, int timeoutSeconds, HttpClient? instanceClient = null)
    {
        var client = instanceClient ?? _staticClient;
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        try
        {
            var response = await client.SendAsync(request, cts.Token);
            if (!response.IsSuccessStatusCode) throw ResponseValidator.GetException(response);
            var content = await response.Content.ReadAsStringAsync(cts.Token);
                    
            if (typeof(T) == typeof(string))
            {
                return new WebApiResponse<T>
                {
                    Content = (T)(object)content,
                    StatusCode = response.StatusCode
                };
            }

            return new WebApiResponse<T>
            {
                Content = JsonConvert.DeserializeObject<T>(content)!,
                StatusCode = response.StatusCode
            };

        }
        catch (OperationCanceledException)
        {
            return new WebApiResponse<T>
            {
                Exception = new TimeoutException("The request timed out."),
                StatusCode = HttpStatusCode.RequestTimeout
            };
        }
    }
}