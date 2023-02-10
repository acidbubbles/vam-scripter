using System;

namespace ScripterLang
{
    public class ScripterRuntimeException : Exception
    {
        public ScripterRuntimeException(string message)
            : base(message)
        {
        }

        public ScripterRuntimeException(string message, Location location)
            : base(message + " (" + location + ")")
        {
        }
    }
}
