using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class UnavailableForLegalReasonsException : Exception
    {
        public UnavailableForLegalReasonsException() { }
        public UnavailableForLegalReasonsException(string message) : base(message) { }
        public UnavailableForLegalReasonsException(string message, Exception inner) : base(message, inner) { }
    }
}
