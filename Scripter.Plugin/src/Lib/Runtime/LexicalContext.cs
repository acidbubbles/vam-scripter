using System;
using System.Collections.Generic;

namespace ScripterLang
{
    public abstract class LexicalContext
    {
        public List<string> Declarations = new List<string>();

        public abstract Func<Value[], Value> GetFunction(string name);
    }
}
