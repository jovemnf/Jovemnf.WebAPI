using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class RequestUriTooLongException : Exception
    {
        public RequestUriTooLongException() { }
        public RequestUriTooLongException(string message) : base(message) { }
        public RequestUriTooLongException(string message, Exception inner) : base(message, inner) { }
    }
}
