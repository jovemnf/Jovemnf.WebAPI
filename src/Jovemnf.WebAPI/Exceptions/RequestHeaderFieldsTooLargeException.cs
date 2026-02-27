using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class RequestHeaderFieldsTooLargeException : Exception
    {
        public RequestHeaderFieldsTooLargeException() { }
        public RequestHeaderFieldsTooLargeException(string message) : base(message) { }
        public RequestHeaderFieldsTooLargeException(string message, Exception inner) : base(message, inner) { }
    }
}
