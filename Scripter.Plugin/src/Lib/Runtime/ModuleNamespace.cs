using System.Collections.Generic;

namespace ScripterLang
{
    public class ModuleNamespace : ObjectReference
    {
        public Value returned;
        public readonly Dictionary<string, Value> exports = new Dictionary<string, Value>();
    }
}
