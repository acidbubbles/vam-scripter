using System;

namespace ScripterLang
{
    public class ScopeLexicalContext : LexicalContext
    {
        private readonly GlobalLexicalContext _root;
        private readonly LexicalContext _parent;

        public ScopeLexicalContext(GlobalLexicalContext parent)
        {
            _root = parent;
            _parent = parent;
        }

        public ScopeLexicalContext(ScopeLexicalContext parent)
        {
            _root = parent._root;
            _parent = parent;
        }

        public override Func<Value[], Value> GetFunction(string name)
        {
            return _root.GetFunction(name);
        }
    }
}