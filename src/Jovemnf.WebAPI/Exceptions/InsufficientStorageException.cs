using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class InsufficientStorageException : Exception
    {
        public InsufficientStorageException() { }
        public InsufficientStorageException(string message) : base(message) { }
        public InsufficientStorageException(string message, Exception inner) : base(message, inner) { }
    }
}
