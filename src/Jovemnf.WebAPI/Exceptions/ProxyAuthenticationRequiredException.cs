using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class ProxyAuthenticationRequiredException : Exception
    {
        public ProxyAuthenticationRequiredException() { }
        public ProxyAuthenticationRequiredException(string message) : base(message) { }
        public ProxyAuthenticationRequiredException(string message, Exception inner) : base(message, inner) { }
    }
}
