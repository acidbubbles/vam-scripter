using System;
using System.Collections.Generic;

namespace ScripterLang
{
    public class GlobalLexicalContext : LexicalContext
    {
        public readonly List<string> StaticDeclarations = new List<string>();
        public readonly Dictionary<string, Func<Value[], Value>> Functions = new Dictionary<string, Func<Value[], Value>>();

        public override Func<Value[], Value> GetFunction(string name)
        {
            return Functions[name];
        }
    }
}
