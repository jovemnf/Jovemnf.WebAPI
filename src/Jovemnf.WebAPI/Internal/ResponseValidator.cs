using System;
using System.Net;
using System.Net.Http;
using Jovemnf.WebAPI.Exceptions;

namespace Jovemnf.WebAPI.Internal
{
    internal static class ResponseValidator
    {
        public static Exception GetException(HttpResponseMessage response)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => new BadRequestException(response.ReasonPhrase ?? ""),
                HttpStatusCode.Unauthorized => new UnauthorizedAccessException(response.ReasonPhrase ?? ""),
                HttpStatusCode.Forbidden => new ForbiddenException(response.ReasonPhrase ?? ""),
                HttpStatusCode.NotFound => new NotFoundException(response.ReasonPhrase ?? ""),
                HttpStatusCode.MethodNotAllowed => new MethodNotAllowedException(response.ReasonPhrase ?? ""),
                HttpStatusCode.NotAcceptable => new NotAcceptableException(response.ReasonPhrase ?? ""),
                HttpStatusCode.ProxyAuthenticationRequired => new ProxyAuthenticationRequiredException(response.ReasonPhrase ?? ""),
                HttpStatusCode.RequestTimeout => new RequestTimeoutException(response.ReasonPhrase ?? ""),
                HttpStatusCode.Conflict => new ConflictException(response.ReasonPhrase ?? ""),
                HttpStatusCode.Gone => new GoneException(response.ReasonPhrase ?? ""),
                HttpStatusCode.LengthRequired => new LengthRequiredException(response.ReasonPhrase ?? ""),
                HttpStatusCode.PreconditionFailed => new PreconditionFailedException(response.ReasonPhrase ?? ""),
                HttpStatusCode.RequestEntityTooLarge => new RequestEntityTooLargeException(response.ReasonPhrase ?? ""),
                HttpStatusCode.RequestUriTooLong => new RequestUriTooLongException(response.ReasonPhrase ?? ""),
                HttpStatusCode.UnsupportedMediaType => new UnsupportedMediaTypeException(response.ReasonPhrase ?? ""),
                HttpStatusCode.RequestedRangeNotSatisfiable => new RequestedRangeNotSatisfiableException(response.ReasonPhrase ?? ""),
                HttpStatusCode.ExpectationFailed => new ExpectationFailedException(response.ReasonPhrase ?? ""),
                HttpStatusCode.MisdirectedRequest => new MisdirectedRequestException(response.ReasonPhrase ?? ""),
                HttpStatusCode.UnprocessableEntity => new UnprocessableEntityException(response.ReasonPhrase ?? ""),
                HttpStatusCode.Locked => new LockedException(response.ReasonPhrase ?? ""),
                HttpStatusCode.FailedDependency => new FailedDependencyException(response.ReasonPhrase ?? ""),
                HttpStatusCode.UpgradeRequired => new UpgradeRequiredException(response.ReasonPhrase ?? ""),
                HttpStatusCode.PreconditionRequired => new PreconditionRequiredException(response.ReasonPhrase ?? ""),
                HttpStatusCode.TooManyRequests => new TooManyRequestsException(response.ReasonPhrase ?? ""),
                HttpStatusCode.RequestHeaderFieldsTooLarge => new RequestHeaderFieldsTooLargeException(response.ReasonPhrase ?? ""),
                HttpStatusCode.UnavailableForLegalReasons => new UnavailableForLegalReasonsException(response.ReasonPhrase ?? ""),
                HttpStatusCode.InternalServerError => new InternalServerError(response.ReasonPhrase ?? ""),
                HttpStatusCode.NotImplemented => new Jovemnf.WebAPI.Exceptions.NotImplementedException(response.ReasonPhrase ?? ""),
                HttpStatusCode.BadGateway => new BadGatewayException(response.ReasonPhrase ?? ""),
                HttpStatusCode.ServiceUnavailable => new ServiceUnavailableException(response.ReasonPhrase ?? ""),
                HttpStatusCode.GatewayTimeout => new GatewayTimeoutException(response.ReasonPhrase ?? ""),
                HttpStatusCode.HttpVersionNotSupported => new HttpVersionNotSupportedException(response.ReasonPhrase ?? ""),
                HttpStatusCode.VariantAlsoNegotiates => new VariantAlsoNegotiatesException(response.ReasonPhrase ?? ""),
                HttpStatusCode.InsufficientStorage => new InsufficientStorageException(response.ReasonPhrase ?? ""),
                HttpStatusCode.LoopDetected => new LoopDetectedException(response.ReasonPhrase ?? ""),
                HttpStatusCode.NotExtended => new NotExtendedException(response.ReasonPhrase ?? ""),
                HttpStatusCode.NetworkAuthenticationRequired => new NetworkAuthenticationRequiredException(response.ReasonPhrase ?? ""),
                HttpStatusCode.PaymentRequired => new PaymentRequired(response.ReasonPhrase ?? ""),
                _ => new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}).")
            };
        }

        public static Exception GetException(WebException e)
        {
            if (e.Status == WebExceptionStatus.ProtocolError && e.Response is HttpWebResponse response)
            {
                return response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => new BadRequestException(response.StatusDescription),
                    HttpStatusCode.Unauthorized => new UnauthorizedAccessException(response.StatusDescription),
                    HttpStatusCode.Forbidden => new ForbiddenException(response.StatusDescription),
                    HttpStatusCode.NotFound => new NotFoundException(response.StatusDescription),
                    HttpStatusCode.MethodNotAllowed => new MethodNotAllowedException(response.StatusDescription),
                    HttpStatusCode.NotAcceptable => new NotAcceptableException(response.StatusDescription),
                    HttpStatusCode.ProxyAuthenticationRequired => new ProxyAuthenticationRequiredException(response.StatusDescription),
                    HttpStatusCode.RequestTimeout => new RequestTimeoutException(response.StatusDescription),
                    HttpStatusCode.Conflict => new ConflictException(response.StatusDescription),
                    HttpStatusCode.Gone => new GoneException(response.StatusDescription),
                    HttpStatusCode.LengthRequired => new LengthRequiredException(response.StatusDescription),
                    HttpStatusCode.PreconditionFailed => new PreconditionFailedException(response.StatusDescription),
                    HttpStatusCode.RequestEntityTooLarge => new RequestEntityTooLargeException(response.StatusDescription),
                    HttpStatusCode.RequestUriTooLong => new RequestUriTooLongException(response.StatusDescription),
                    HttpStatusCode.UnsupportedMediaType => new UnsupportedMediaTypeException(response.StatusDescription),
                    HttpStatusCode.RequestedRangeNotSatisfiable => new RequestedRangeNotSatisfiableException(response.StatusDescription),
                    HttpStatusCode.ExpectationFailed => new ExpectationFailedException(response.StatusDescription),
                    HttpStatusCode.MisdirectedRequest => new MisdirectedRequestException(response.StatusDescription),
                    HttpStatusCode.UnprocessableEntity => new UnprocessableEntityException(response.StatusDescription),
                    HttpStatusCode.Locked => new LockedException(response.StatusDescription),
                    HttpStatusCode.FailedDependency => new FailedDependencyException(response.StatusDescription),
                    HttpStatusCode.UpgradeRequired => new UpgradeRequiredException(response.StatusDescription),
                    HttpStatusCode.PreconditionRequired => new PreconditionRequiredException(response.StatusDescription),
                    HttpStatusCode.TooManyRequests => new TooManyRequestsException(response.StatusDescription),
                    HttpStatusCode.RequestHeaderFieldsTooLarge => new RequestHeaderFieldsTooLargeException(response.StatusDescription),
                    HttpStatusCode.UnavailableForLegalReasons => new UnavailableForLegalReasonsException(response.StatusDescription),
                    HttpStatusCode.InternalServerError => new InternalServerError(response.StatusDescription),
                    HttpStatusCode.NotImplemented => new Jovemnf.WebAPI.Exceptions.NotImplementedException(response.StatusDescription),
                    HttpStatusCode.BadGateway => new BadGatewayException(response.StatusDescription),
                    HttpStatusCode.ServiceUnavailable => new ServiceUnavailableException(response.StatusDescription),
                    HttpStatusCode.GatewayTimeout => new GatewayTimeoutException(response.StatusDescription),
                    HttpStatusCode.HttpVersionNotSupported => new HttpVersionNotSupportedException(response.StatusDescription),
                    HttpStatusCode.VariantAlsoNegotiates => new VariantAlsoNegotiatesException(response.StatusDescription),
                    HttpStatusCode.InsufficientStorage => new InsufficientStorageException(response.StatusDescription),
                    HttpStatusCode.LoopDetected => new LoopDetectedException(response.StatusDescription),
                    HttpStatusCode.NotExtended => new NotExtendedException(response.StatusDescription),
                    HttpStatusCode.NetworkAuthenticationRequired => new NetworkAuthenticationRequiredException(response.StatusDescription),
                    HttpStatusCode.PaymentRequired => new PaymentRequired(response.StatusDescription),
                    _ => (Exception)e
                };
            }
            
            if (e.Status == WebExceptionStatus.Timeout)
            {
                return new TimeoutException();
            }
            
            if (e.Status == WebExceptionStatus.RequestProhibitedByProxy)
            {
                return new ProxyProhibited();
            }

            return e;
        }
    }
}
