using System;
using System.Collections.Generic;

namespace ScripterLang
{
    public class RuntimeLexicalContext
    {
        public readonly Dictionary<string, Value> Variables = new Dictionary<string, Value>();

        public Func<Value[], Value> GetFunction(LexicalContext lexicalContext, string name)
        {
            return lexicalContext.GetFunction(name);
        }
    }
}
