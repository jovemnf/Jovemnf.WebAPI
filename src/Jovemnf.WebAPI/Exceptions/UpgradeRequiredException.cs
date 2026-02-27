using System;

namespace Jovemnf.WebAPI.Exceptions
{
    public class UpgradeRequiredException : Exception
    {
        public UpgradeRequiredException() { }
        public UpgradeRequiredException(string message) : base(message) { }
        public UpgradeRequiredException(string message, Exception inner) : base(message, inner) { }
    }
}
