using System.Collections.Generic;

namespace ScripterLang
{
    public class GlobalLexicalContext : ScopeLexicalContext
    {
        private readonly Dictionary<string, ModuleLexicalContext> _modules = new Dictionary<string, ModuleLexicalContext>();

        public GlobalLexicalContext() : base(null)
        {
        }

        public void DeclareModule(string module, ModuleLexicalContext context)
        {
            #warning Dispose modules
            _modules[module] = context;
        }

        public ModuleLexicalContext GetModule(string module)
        {
            ModuleLexicalContext context;
            if (_modules.TryGetValue(module, out context))
                return context;
            throw new ScripterRuntimeException($"Module {module} was not declared");
        }

        public override GlobalLexicalContext GetGlobalContext() => this;

        public override Value SetVariableValue(string name, Value value)
        {
            throw new ScripterRuntimeException("Cannot set variable value in the global context");
        }

        public override ModuleLexicalContext GetModuleContext()
        {
            throw new ScripterRuntimeException("Cannot access module context from the global context");
        }

        public override FunctionLexicalContext GetFunctionContext()
        {
            throw new ScripterRuntimeException("Cannot access function context from the global context");
        }
    }
}
