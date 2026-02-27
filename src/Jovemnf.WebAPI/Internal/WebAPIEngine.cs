using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Jovemnf.WebAPI.Internal
{
    internal class WebAPIEngine
    {
        private static HttpClient _staticClient = new HttpClient();

        public static void SetClient(HttpClient client) => _staticClient = client;

        public async Task<T> SendAsync<T>(HttpRequestMessage request, int timeoutSeconds, HttpClient? instanceClient = null)
        {
            var client = instanceClient ?? _staticClient;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            try
            {
                var response = await client.SendAsync(request, cts.Token);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    
                    if (typeof(T) == typeof(string))
                    {
                        return (T)(object)content;
                    }

                    return JsonConvert.DeserializeObject<T>(content)!;
                }

                throw ResponseValidator.GetException(response);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("The request timed out.");
            }
        }

        public async Task<string> SendAndGetStringAsync(HttpRequestMessage request, int timeoutSeconds, HttpClient? instanceClient = null)
        {
            var client = instanceClient ?? _staticClient;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            try
            {
                var response = await client.SendAsync(request, cts.Token);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                throw ResponseValidator.GetException(response);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("The request timed out.");
            }
        }
    }
}
