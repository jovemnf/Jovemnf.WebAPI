using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class VariantAlsoNegotiatesException : Exception
    {
        public VariantAlsoNegotiatesException() { }
        public VariantAlsoNegotiatesException(string message) : base(message) { }
        public VariantAlsoNegotiatesException(string message, Exception inner) : base(message, inner) { }
    }
}
