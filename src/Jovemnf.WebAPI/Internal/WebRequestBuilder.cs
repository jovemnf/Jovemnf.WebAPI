using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Jovemnf.WebAPI.Internal
{
    internal class WebRequestBuilder
    {
        private readonly Uri _uri;
        private MethodRequest _method = MethodRequest.GET;
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
        private HttpContent? _content;

        public WebRequestBuilder(Uri uri)
        {
            _uri = uri;
        }

        public WebRequestBuilder WithMethod(MethodRequest method)
        {
            _method = method;
            return this;
        }

        public WebRequestBuilder WithHeader(string key, string value)
        {
            _headers[key] = value;
            return this;
        }

        public WebRequestBuilder WithHeaders(IEnumerable<KeyValuePair<string, string>> headers)
        {
            foreach (var header in headers)
            {
                _headers[header.Key] = header.Value;
            }
            return this;
        }

        public WebRequestBuilder WithContent(HttpContent? content)
        {
            _content = content;
            return this;
        }

        public HttpRequestMessage Build()
        {
            var request = new HttpRequestMessage(GetHttpMethod(_method), _uri);

            foreach (var header in _headers)
            {
                // Special handling for Authorization header to ensure proper parsing
                if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = header.Value.Split(' ', 2);
                    if (parts.Length == 2)
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue(parts[0], parts[1]);
                        continue;
                    }
                }

                if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    // If it fails to add to request headers, it might be a content header but 
                    // content headers can only be added if _content is already assigned. 
                    // For compatibility with the legacy mixed header approach, we prioritize request headers.
                }
            }

            if (_content != null)
            {
                request.Content = _content;
            }

            return request;
        }

        private static HttpMethod GetHttpMethod(MethodRequest method)
        {
            return method switch
            {
                MethodRequest.POST => HttpMethod.Post,
                MethodRequest.GET => HttpMethod.Get,
                MethodRequest.DELETE => HttpMethod.Delete,
                MethodRequest.PUT => HttpMethod.Put,
                MethodRequest.PATCH => HttpMethod.Patch,
                _ => HttpMethod.Get
            };
        }
    }
}
