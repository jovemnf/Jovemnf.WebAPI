using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class HttpVersionNotSupportedException : Exception
    {
        public HttpVersionNotSupportedException() { }
        public HttpVersionNotSupportedException(string message) : base(message) { }
        public HttpVersionNotSupportedException(string message, Exception inner) : base(message, inner) { }
    }
}
