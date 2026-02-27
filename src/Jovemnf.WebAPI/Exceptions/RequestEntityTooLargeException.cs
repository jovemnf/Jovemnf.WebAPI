using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class RequestEntityTooLargeException : Exception
    {
        public RequestEntityTooLargeException() { }
        public RequestEntityTooLargeException(string message) : base(message) { }
        public RequestEntityTooLargeException(string message, Exception inner) : base(message, inner) { }
    }
}
