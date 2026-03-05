using System.Net;
using System.Threading;
using Jovemnf.WebApi;
using Jovemnf.WebAPI.Exceptions;

namespace Jovemnf.WebAPI.Tests
{
    public class WebApiTests
    {
        private class MockHttpMessageHandler : HttpMessageHandler
        {
            public Func<HttpRequestMessage, HttpResponseMessage>? ResponseFactory { get; set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (ResponseFactory == null) return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
                return Task.FromResult(ResponseFactory(request));
            }
        }

        #region Helpers

        private HttpClient CreateMockClient(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            var handler = new MockHttpMessageHandler { ResponseFactory = responseFactory };
            return new HttpClient(handler);
        }

        #endregion

        #region Instance Methods Tests

        /*
        [Fact]
        public async Task SendAndGetString_ShouldReturnContent_OnSuccess()
        {
            // Arrange
            var expectedContent = "{\"status\":\"ok\"}";
            var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(expectedContent) });
            var api = new Jovemnf.WebAPI.WebApi(new Uri("https://api.example.com"), client);

            // Act
            var result = await api.SendAndGetString();

            // Assert
            Assert.Equal(expectedContent, result);
        }
        */

        [Fact]
        public async Task SetJson_ShouldSendCorrectContent()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;
            string? capturedContent = null;
            var client = CreateMockClient(req =>
            {
                capturedRequest = req;
                capturedContent = req.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") };
            });
            var api = new Api(new Uri("https://api.example.com"), client);
            var data = new { Name = "Test", Value = 123 };

            // Act
            api.SetJson(data);
            await api.Send();

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal(HttpMethod.Post, capturedRequest.Method);
            Assert.Equal("application/json", capturedRequest.Content?.Headers.ContentType?.MediaType);
            Assert.Contains("\"Name\":\"Test\"", capturedContent);
        }

        [Fact]
        public async Task SetDataPost_ShouldSendUrlEncodedForm()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;
            string? capturedContent = null;
            var client = CreateMockClient(req =>
            {
                capturedRequest = req;
                capturedContent = req.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") };
            });
            var api = new Api(new Uri("https://api.example.com"), client);
            var data = new Dictionary<string, object> { { "key", "value" } };

            // Act
            api.SetDataPost(data);
            await api.Send();

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal("application/x-www-form-urlencoded", capturedRequest.Content?.Headers.ContentType?.MediaType);
            Assert.Equal("key=value", capturedContent);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound, typeof(NotFoundException))]
        [InlineData(HttpStatusCode.Unauthorized, typeof(UnauthorizedAccessException))]
        [InlineData(HttpStatusCode.InternalServerError, typeof(InternalServerError))]
        [InlineData(HttpStatusCode.PaymentRequired, typeof(PaymentRequired))]
        public async Task ErrorHandling_ShouldThrowCorrectExceptions(HttpStatusCode statusCode, Type expectedExceptionType)
        {
            // Arrange
            var client = CreateMockClient(req => new HttpResponseMessage(statusCode));
            var api = new Api(new Uri("https://api.example.com"), client);

            // Act & Assert
            await Assert.ThrowsAsync(expectedExceptionType, async () => await api.Send());
        }

        [Fact]
        public async Task SetHeader_ShouldApplyHeaders()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;
            var client = CreateMockClient(req => { capturedRequest = req; return new HttpResponseMessage(HttpStatusCode.OK); });
            var api = new Api(new Uri("https://api.example.com"), client, 10000, MethodRequest.PATCH);

            // Act
            api.SetHeader("X-Test", "Value");
            await api.Send();

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.True(capturedRequest.Headers.Contains("X-Test"));
        }

        [Fact]
        public async Task WithHttpClient_ShouldOverrideEngineClient()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;
            var client = CreateMockClient(req => { capturedRequest = req; return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }; });
            var api = new Api("https://api.example.com")
                .WithHttpClient(client);

            // Act
            await api.Send();

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal("https://api.example.com/", capturedRequest.RequestUri?.ToString());
        }

        [Fact]
        public async Task WithBasicAuth_ShouldApplyCorrectHeader()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;
            var client = CreateMockClient(req => 
            { 
                capturedRequest = req; 
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }; 
            });
            
            var api = new Api("https://api.example.com")
                .WithHttpClient(client)
                .WithBasicAuth("user", "pass");

            // Act
            await api.Send();

            // Assert
            Assert.NotNull(capturedRequest);
            var authHeader = capturedRequest.Headers.Authorization;
            Assert.NotNull(authHeader);
            Assert.Equal("Basic", authHeader.Scheme);
            Assert.Equal(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("user:pass")), authHeader.Parameter);
        }

        [Fact]
        public void WithCertificate_ShouldSuccessfullyInitializeHttpClient()
        {
            // Arrange
            using var rsa = System.Security.Cryptography.RSA.Create(2048);
            var request = new System.Security.Cryptography.X509Certificates.CertificateRequest(
                "cn=test", rsa, System.Security.Cryptography.HashAlgorithmName.SHA256, System.Security.Cryptography.RSASignaturePadding.Pkcs1);
            using var cert = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

            var api = new Api("https://api.example.com");

            // Act
            var result = api.WithCertificate(cert);

            // Assert
            Assert.Same(api, result);
            // Verification that it doesn't throw and remains fluent
        }

        #endregion

        #region Static API Tests

        [Fact]
        public async Task Static_Get_ShouldInvokeCorrectRequest()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;
            var client = CreateMockClient(req => { capturedRequest = req; return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }; });
            Api.SetStaticClient(client);

            // Act
            await Api.Get("https://api.example.com/static-get");

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal(HttpMethod.Get, capturedRequest.Method);
            Assert.Equal("https://api.example.com/static-get", capturedRequest.RequestUri?.ToString());
        }

        [Fact]
        public async Task Static_Post_ShouldSendJson()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;
            string? capturedContent = null;
            var client = CreateMockClient(req =>
            {
                capturedRequest = req;
                capturedContent = req.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") };
            });
            Api.SetStaticClient(client);

            // Act
            await Api.Post("https://api.example.com/static-post", new { Foo = "Bar" });

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal(HttpMethod.Post, capturedRequest.Method);
            Assert.Contains("\"Foo\":\"Bar\"", capturedContent);
        }

        [Fact]
        public async Task Static_Patch_ShouldUseCorrectVerb()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;
            var client = CreateMockClient(req => { capturedRequest = req; return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }; });
            Api.SetStaticClient(client);

            // Act
            await Api.Patch<object>("https://api.example.com/patch", new { val = 1 });

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal("PATCH", capturedRequest.Method.Method);
        }

        [Fact]
        public async Task Static_Put_ShouldSendJsonAndUseCorrectVerb()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;
            string? capturedContent = null;
            var client = CreateMockClient(req =>
            {
                capturedRequest = req;
                capturedContent = req.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"id\":1}") };
            });
            Api.SetStaticClient(client);

            // Act
            var response = await Api.Put<FakeDto>("https://api.example.com/put", new { name = "Updated" });

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal(HttpMethod.Put, capturedRequest.Method);
            Assert.Contains("\"name\":\"Updated\"", capturedContent);
            Assert.NotNull(response.Content);
            Assert.Equal(1, response.Content.Id);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Static_Delete_ShouldInvokeCorrectRequestAndReturnResponse()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;
            var client = CreateMockClient(req =>
            {
                capturedRequest = req;
                return new HttpResponseMessage(HttpStatusCode.NoContent) { Content = new StringContent("") };
            });
            Api.SetStaticClient(client);

            // Act
            var response = await Api.Delete("https://api.example.com/delete/1");

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal(HttpMethod.Delete, capturedRequest.Method);
            Assert.Equal("https://api.example.com/delete/1", capturedRequest.RequestUri?.ToString());
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Static_Get_WithHeaders_ShouldSendHeaders()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;
            var client = CreateMockClient(req =>
            {
                capturedRequest = req;
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") };
            });
            Api.SetStaticClient(client);
            var headers = new Dictionary<string, string> { { "X-Custom", "Value" }, { "Accept", "application/json" } };

            // Act
            await Api.Get("https://api.example.com/with-headers", headers);

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.True(capturedRequest.Headers.Contains("X-Custom"));
            Assert.Equal("Value", capturedRequest.Headers.GetValues("X-Custom").FirstOrDefault());
        }

        [Fact]
        public async Task Send_ShouldReturnWebApiResponseWithContentAndStatusCode()
        {
            // Arrange
            var json = "{\"id\":42,\"name\":\"Test\"}";
            var client = CreateMockClient(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });
            var api = new Api(new Uri("https://api.example.com"), client);

            // Act
            var response = await api.Send<FakeDto>();

            // Assert
            Assert.NotNull(response.Content);
            Assert.Equal(42, response.Content.Id);
            Assert.Equal("Test", response.Content.Name);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Null(response.Exception);
        }

        [Fact]
        public async Task Send_GetString_ShouldReturnStringContent()
        {
            // Arrange
            var raw = "plain text";
            var client = CreateMockClient(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(raw) });
            var api = new Api(new Uri("https://api.example.com"), client);

            // Act
            var response = await api.Send<string>();

            // Assert
            Assert.Equal(raw, response.Content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Timeout_ShouldReturnWebApiResponseWithException()
        {
            // Arrange: handler que demora mais que o timeout (3s) para disparar cancelamento (timeout 1s)
            var client = CreateMockClient(_ =>
            {
                Thread.Sleep(3000);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") };
            });
            var shortTimeoutApi = new Api(new Uri("https://api.example.com"), client, 1000, MethodRequest.GET);

            // Act
            var response = await shortTimeoutApi.Send<object>();

            // Assert
            Assert.NotNull(response.Exception);
            Assert.IsType<TimeoutException>(response.Exception);
            Assert.Equal(HttpStatusCode.RequestTimeout, response.StatusCode);
        }

        [Fact]
        public async Task WithHeaders_ShouldApplyAllHeaders()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;
            var client = CreateMockClient(req => { capturedRequest = req; return new HttpResponseMessage(HttpStatusCode.OK); });
            var api = new Api(new Uri("https://api.example.com"), client);
            var headers = new Dictionary<string, string> { { "A", "1" }, { "B", "2" } };

            // Act
            api.WithHeaders(headers);
            await api.Send();

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.True(capturedRequest.Headers.Contains("A"));
            Assert.True(capturedRequest.Headers.Contains("B"));
        }

        #endregion
    }

    internal class FakeDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
