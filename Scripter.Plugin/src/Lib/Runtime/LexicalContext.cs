using System;
using System.Collections.Generic;

namespace ScripterLang
{
    public abstract class LexicalContext
    {
        public readonly List<string> Declarations = new List<string>();

        public abstract Func<RuntimeDomain, Value[], Value> GetFunction(string name);

        public virtual void Declare(string name, Location location)
        {
            if (Declarations.Contains(name))
                throw new ScripterParsingException($"Variable {name} was already declared", location);
        }
    }
}
