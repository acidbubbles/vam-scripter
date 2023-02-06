using System;

namespace ScripterLang
{
    public abstract class LexicalContext
    {
        public abstract Func<RuntimeDomain, Value[], Value> GetFunction(string name);
    }
}
