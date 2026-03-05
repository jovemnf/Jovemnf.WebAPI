using System;
using System.Net;
using System.Net.Http;
using Jovemnf.WebApi;
using Xunit;
using Jovemnf.WebAPI.Exceptions;

namespace Jovemnf.WebAPI.Tests
{
    public class ExceptionTests
    {
        [Theory]
        [InlineData(HttpStatusCode.BadRequest, typeof(BadRequestException))]
        [InlineData(HttpStatusCode.Forbidden, typeof(ForbiddenException))]
        [InlineData(HttpStatusCode.MethodNotAllowed, typeof(MethodNotAllowedException))]
        [InlineData(HttpStatusCode.NotAcceptable, typeof(NotAcceptableException))]
        [InlineData(HttpStatusCode.ProxyAuthenticationRequired, typeof(ProxyAuthenticationRequiredException))]
        [InlineData(HttpStatusCode.RequestTimeout, typeof(RequestTimeoutException))]
        [InlineData(HttpStatusCode.Conflict, typeof(ConflictException))]
        [InlineData(HttpStatusCode.Gone, typeof(GoneException))]
        [InlineData(HttpStatusCode.LengthRequired, typeof(LengthRequiredException))]
        [InlineData(HttpStatusCode.PreconditionFailed, typeof(PreconditionFailedException))]
        [InlineData(HttpStatusCode.RequestEntityTooLarge, typeof(RequestEntityTooLargeException))]
        [InlineData(HttpStatusCode.RequestUriTooLong, typeof(RequestUriTooLongException))]
        [InlineData(HttpStatusCode.UnsupportedMediaType, typeof(UnsupportedMediaTypeException))]
        [InlineData(HttpStatusCode.RequestedRangeNotSatisfiable, typeof(RequestedRangeNotSatisfiableException))]
        [InlineData(HttpStatusCode.ExpectationFailed, typeof(ExpectationFailedException))]
        [InlineData(HttpStatusCode.MisdirectedRequest, typeof(MisdirectedRequestException))]
        [InlineData(HttpStatusCode.UnprocessableEntity, typeof(UnprocessableEntityException))]
        [InlineData(HttpStatusCode.Locked, typeof(LockedException))]
        [InlineData(HttpStatusCode.FailedDependency, typeof(FailedDependencyException))]
        [InlineData(HttpStatusCode.UpgradeRequired, typeof(UpgradeRequiredException))]
        [InlineData(HttpStatusCode.PreconditionRequired, typeof(PreconditionRequiredException))]
        [InlineData(HttpStatusCode.TooManyRequests, typeof(TooManyRequestsException))]
        [InlineData(HttpStatusCode.RequestHeaderFieldsTooLarge, typeof(RequestHeaderFieldsTooLargeException))]
        [InlineData(HttpStatusCode.UnavailableForLegalReasons, typeof(UnavailableForLegalReasonsException))]
        [InlineData(HttpStatusCode.InternalServerError, typeof(InternalServerError))]
        [InlineData(HttpStatusCode.NotImplemented, typeof(Jovemnf.WebAPI.Exceptions.NotImplementedException))]
        [InlineData(HttpStatusCode.BadGateway, typeof(BadGatewayException))]
        [InlineData(HttpStatusCode.ServiceUnavailable, typeof(ServiceUnavailableException))]
        [InlineData(HttpStatusCode.GatewayTimeout, typeof(GatewayTimeoutException))]
        [InlineData(HttpStatusCode.HttpVersionNotSupported, typeof(HttpVersionNotSupportedException))]
        [InlineData(HttpStatusCode.VariantAlsoNegotiates, typeof(VariantAlsoNegotiatesException))]
        [InlineData(HttpStatusCode.InsufficientStorage, typeof(InsufficientStorageException))]
        [InlineData(HttpStatusCode.LoopDetected, typeof(LoopDetectedException))]
        [InlineData(HttpStatusCode.NotExtended, typeof(NotExtendedException))]
        [InlineData(HttpStatusCode.NetworkAuthenticationRequired, typeof(NetworkAuthenticationRequiredException))]
        [InlineData(HttpStatusCode.PaymentRequired, typeof(PaymentRequired))]
        [InlineData(HttpStatusCode.NotFound, typeof(NotFoundException))]
        public void CheckException_ShouldMapToCorrectException(HttpStatusCode statusCode, Type expectedType)
        {
            // Arrange
            var response = new HttpResponseMessage(statusCode);

            // Act
            var exception = Api.CheckException(response);

            // Assert
            Assert.IsType(expectedType, exception);
        }

        [Fact]
        public void CheckException_WebException_Timeout_ShouldReturnTimeoutException()
        {
            var webEx = new WebException("Timeout", null, WebExceptionStatus.Timeout, null);

            var result = Api.CheckException(webEx);

            Assert.IsType<TimeoutException>(result);
        }

        [Fact]
        public void CheckException_WebException_ProxyProhibited_ShouldReturnProxyProhibited()
        {
            var webEx = new WebException("Proxy", null, WebExceptionStatus.RequestProhibitedByProxy, null);

            var result = Api.CheckException(webEx);

            Assert.IsType<ProxyProhibited>(result);
        }

        [Fact]
        public void CheckException_HttpResponseMessage_Unauthorized_ShouldIncludeMessage()
        {
            var msg = "Invalid token";
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = msg };
            var exception = Api.CheckException(response) as UnauthorizedAccessException;

            Assert.NotNull(exception);
            Assert.Equal(msg, exception.Message);
        }
    }
}
