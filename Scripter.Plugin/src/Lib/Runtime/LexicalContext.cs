using System;
using System.Collections.Generic;

namespace ScripterLang
{
    public class LexicalContext
    {
        public readonly Dictionary<string, Value> Variables = new Dictionary<string, Value>();
        public readonly Dictionary<string, Func<Value[], Value>> Functions = new Dictionary<string, Func<Value[], Value>>();
    }
}
