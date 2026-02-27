using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class LengthRequiredException : Exception
    {
        public LengthRequiredException() { }
        public LengthRequiredException(string message) : base(message) { }
        public LengthRequiredException(string message, Exception inner) : base(message, inner) { }
    }
}
