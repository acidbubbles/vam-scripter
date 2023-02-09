using System.Collections.Generic;

namespace ScripterLang
{
    public class GlobalLexicalContext : ScopeLexicalContext
    {
        private readonly Dictionary<string, ModuleExpression> _modules = new Dictionary<string, ModuleExpression>();

        public GlobalLexicalContext() : base(null)
        {
        }

        public void DeclareModule(string module, ModuleExpression context)
        {
            RemoveModule(module);
            _modules[module] = context;
        }

        public void RemoveModule(string moduleName)
        {
            ModuleExpression module;
            if (!_modules.TryGetValue(moduleName, out module))
                return;
            _modules.Remove(moduleName);
        }

        public ModuleExpression GetModule(string module)
        {
            ModuleExpression context;
            if (_modules.TryGetValue(module, out context))
                return context;
            throw new ScripterRuntimeException($"Module {module} was not declared");
        }

        public override GlobalLexicalContext GetGlobalContext() => this;

        public override Value SetVariableValue(string name, Value value)
        {
            throw new ScripterRuntimeException($"{name} was not defined");
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
