using System;
using System.Collections.Generic;

namespace ScripterLang
{
    public abstract class LexicalContext
    {
        public readonly Dictionary<string, Value> Declarations = new Dictionary<string, Value>();

        public abstract Func<Value[], Value> GetFunction(string name);
    }
}
