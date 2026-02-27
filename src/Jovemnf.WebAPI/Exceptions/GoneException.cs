using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class GoneException : Exception
    {
        public GoneException() { }
        public GoneException(string message) : base(message) { }
        public GoneException(string message, Exception inner) : base(message, inner) { }
    }
}
