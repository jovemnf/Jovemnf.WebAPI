using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class NotAcceptableException : Exception
    {
        public NotAcceptableException() { }
        public NotAcceptableException(string message) : base(message) { }
        public NotAcceptableException(string message, Exception inner) : base(message, inner) { }
    }
}
