using System;

namespace ScripterLang
{
    public abstract class LexicalContext
    {
        public abstract Func<Value[], Value> GetFunction(string name);
    }
}
