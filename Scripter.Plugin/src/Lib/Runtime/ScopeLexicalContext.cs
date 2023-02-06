using System;

namespace ScripterLang
{
    public class ScopeLexicalContext : LexicalContext
    {
        public readonly GlobalLexicalContext Root;

        private readonly LexicalContext _parent;

        public ScopeLexicalContext(GlobalLexicalContext root)
        {
            Root = root;
            _parent = root;
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

        public override void Declare(string name, Location location)
        {
            LexicalContext scope = this;
            do
            {
                if (scope.Declarations.Contains(name))
                    throw new ScripterParsingException($"Variable {name} was already declared in an outer scope", location);
                scope = (scope as ScopeLexicalContext)?._parent;
            } while (scope != null);

            Declarations.Add(name);
        }
    }
}
