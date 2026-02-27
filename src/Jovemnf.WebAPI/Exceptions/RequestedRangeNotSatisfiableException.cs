using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class RequestedRangeNotSatisfiableException : Exception
    {
        public RequestedRangeNotSatisfiableException() { }
        public RequestedRangeNotSatisfiableException(string message) : base(message) { }
        public RequestedRangeNotSatisfiableException(string message, Exception inner) : base(message, inner) { }
    }
}
