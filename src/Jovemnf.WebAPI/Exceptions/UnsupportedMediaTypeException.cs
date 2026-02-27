using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class UnsupportedMediaTypeException : Exception
    {
        public UnsupportedMediaTypeException() { }
        public UnsupportedMediaTypeException(string message) : base(message) { }
        public UnsupportedMediaTypeException(string message, Exception inner) : base(message, inner) { }
    }
}
