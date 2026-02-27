using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class RequestTimeoutException : Exception
    {
        public RequestTimeoutException() { }
        public RequestTimeoutException(string message) : base(message) { }
        public RequestTimeoutException(string message, Exception inner) : base(message, inner) { }
    }
}
