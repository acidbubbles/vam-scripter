using System.Collections.Generic;

namespace ScripterLang
{
    public class GlobalLexicalContext : LexicalContext
    {
        public readonly Dictionary<string, Value> Globals = new Dictionary<string, Value>();
    }
}
