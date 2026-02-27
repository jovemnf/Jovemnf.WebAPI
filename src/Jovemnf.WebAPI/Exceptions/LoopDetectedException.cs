using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class LoopDetectedException : Exception
    {
        public LoopDetectedException() { }
        public LoopDetectedException(string message) : base(message) { }
        public LoopDetectedException(string message, Exception inner) : base(message, inner) { }
    }
}
