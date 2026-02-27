using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class MethodNotAllowedException : Exception
    {
        public MethodNotAllowedException() { }
        public MethodNotAllowedException(string message) : base(message) { }
        public MethodNotAllowedException(string message, Exception inner) : base(message, inner) { }
    }
}
