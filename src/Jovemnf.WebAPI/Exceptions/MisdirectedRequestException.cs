using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class MisdirectedRequestException : Exception
    {
        public MisdirectedRequestException() { }
        public MisdirectedRequestException(string message) : base(message) { }
        public MisdirectedRequestException(string message, Exception inner) : base(message, inner) { }
    }
}
