using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Jovemnf.WebAPI.Exceptions;

namespace Jovemnf.WebAPI.Tests
{
    public class WebAPITests
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

        [Fact]
        public async Task SendAndGetString_ShouldReturnContent_OnSuccess()
        {
            // Arrange
            var expectedContent = "{\"status\":\"ok\"}";
            var client = CreateMockClient(req => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(expectedContent) });
            var api = new Jovemnf.WebAPI.WebAPI(new Uri("https://api.example.com"), client);

            // Act
            var result = await api.SendAndGetString();

            // Assert
            Assert.Equal(expectedContent, result);
        }

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
            var api = new Jovemnf.WebAPI.WebAPI(new Uri("https://api.example.com"), client);
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
            var api = new Jovemnf.WebAPI.WebAPI(new Uri("https://api.example.com"), client);
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
            var api = new Jovemnf.WebAPI.WebAPI(new Uri("https://api.example.com"), client);

            // Act & Assert
            await Assert.ThrowsAsync(expectedExceptionType, async () => await api.Send());
        }

        [Fact]
        public async Task SetHeader_ShouldApplyHeaders()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;
            var client = CreateMockClient(req => { capturedRequest = req; return new HttpResponseMessage(HttpStatusCode.OK); });
            var api = new Jovemnf.WebAPI.WebAPI(new Uri("https://api.example.com"), client, 10000, Jovemnf.WebAPI.WebAPI.MethodRequest.PATCH);

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
            var api = new Jovemnf.WebAPI.WebAPI("https://api.example.com")
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
            
            var api = new Jovemnf.WebAPI.WebAPI("https://api.example.com")
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

            var api = new Jovemnf.WebAPI.WebAPI("https://api.example.com");

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
            Jovemnf.WebAPI.WebAPI.SetStaticClient(client);

            // Act
            await Jovemnf.WebAPI.WebAPI.Get("https://api.example.com/static-get");

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
            Jovemnf.WebAPI.WebAPI.SetStaticClient(client);

            // Act
            await Jovemnf.WebAPI.WebAPI.Post("https://api.example.com/static-post", new { Foo = "Bar" });

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
            Jovemnf.WebAPI.WebAPI.SetStaticClient(client);

            // Act
            await Jovemnf.WebAPI.WebAPI.Patch<object>("https://api.example.com/patch", new { val = 1 });

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal("PATCH", capturedRequest.Method.Method);
        }

        #endregion
    }
}
