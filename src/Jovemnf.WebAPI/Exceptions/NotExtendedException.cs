using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class NotExtendedException : Exception
    {
        public NotExtendedException() { }
        public NotExtendedException(string message) : base(message) { }
        public NotExtendedException(string message, Exception inner) : base(message, inner) { }
    }
}
