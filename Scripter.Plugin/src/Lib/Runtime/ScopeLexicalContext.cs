using System;
using System.Collections.Generic;

namespace ScripterLang
{
    public class ScopeLexicalContext : LexicalContext
    {
        public readonly List<string> Declarations = new List<string>();
        public readonly GlobalLexicalContext Root;

        private readonly LexicalContext _parent;

        public ScopeLexicalContext(GlobalLexicalContext parent)
        {
            Root = parent;
            _parent = parent;
        }

        public ScopeLexicalContext(ScopeLexicalContext parent)
        {
            Root = parent.Root;
            _parent = parent;
        }

        public override Func<Value[], Value> GetFunction(string name)
        {
            return Root.GetFunction(name);
        }
    }
}
