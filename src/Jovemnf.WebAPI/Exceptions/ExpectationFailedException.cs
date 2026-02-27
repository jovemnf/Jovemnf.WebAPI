using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class ExpectationFailedException : Exception
    {
        public ExpectationFailedException() { }
        public ExpectationFailedException(string message) : base(message) { }
        public ExpectationFailedException(string message, Exception inner) : base(message, inner) { }
    }
}
