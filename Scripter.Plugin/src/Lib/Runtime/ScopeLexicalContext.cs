using System;
using System.Collections.Generic;

namespace ScripterLang
{
    public class ScopeLexicalContext : LexicalContext
    {
        public readonly List<string> Declarations = new List<string>();
        public readonly GlobalLexicalContext Root;

        private readonly ScopeLexicalContext _parent;

        public ScopeLexicalContext(GlobalLexicalContext root)
        {
            Root = root;
        }

        public ScopeLexicalContext(ScopeLexicalContext parent)
        {
            Root = parent.Root;
            _parent = parent;
        }

        public override Func<RuntimeDomain, Value[], Value> GetFunction(string name)
        {
            return Root.GetFunction(name);
        }

        public void Declare(string name, Location location)
        {
            var scope = this;
            do
            {
                if (scope.Declarations.Contains(name))
                    throw new ScripterParsingException($"Variable {name} was already declared in an outer scope", location);
                Declarations.Add(name);

            } while ((scope = scope._parent) != null);
        }
    }
}
