using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class PreconditionRequiredException : Exception
    {
        public PreconditionRequiredException() { }
        public PreconditionRequiredException(string message) : base(message) { }
        public PreconditionRequiredException(string message, Exception inner) : base(message, inner) { }
    }
}
