using System;

namespace ScripterLang
{
    public class ScripterParsingException : Exception
    {
        public ScripterParsingException(string message)
            : base(message)
        {
        }
    }
}
