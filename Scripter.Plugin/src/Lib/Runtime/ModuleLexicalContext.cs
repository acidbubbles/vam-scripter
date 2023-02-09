using System.Collections.Generic;

namespace ScripterLang
{
    public class ModuleLexicalContext : FunctionLexicalContext
    {
        public readonly GlobalLexicalContext GlobalContext;

        public readonly Dictionary<string, Value> Exports = new Dictionary<string, Value>();

        public ModuleLexicalContext(GlobalLexicalContext globalContext)
            : base(globalContext)
        {
            GlobalContext = globalContext;
        }

        public override GlobalLexicalContext GetGlobalContext() => GlobalContext;
        public override ModuleLexicalContext GetModuleContext() => this;
    }
}
