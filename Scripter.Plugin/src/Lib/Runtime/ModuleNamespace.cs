using System.Collections.Generic;

namespace ScripterLang
{
    public class ModuleNamespace : ObjectReference
    {
        public Value Returned;
        public readonly Dictionary<string, Value> Exports = new Dictionary<string, Value>();
    }
}
