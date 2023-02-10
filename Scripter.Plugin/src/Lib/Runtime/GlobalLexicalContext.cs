using System.Collections.Generic;

namespace ScripterLang
{
    public class GlobalLexicalContext : ScopeLexicalContext
    {
        private readonly Dictionary<string, IModule> _modules = new Dictionary<string, IModule>();

        public GlobalLexicalContext() : base(null)
        {
        }

        public void DeclareGlobal(string name, Value value)
        {
            Declare(new VariableReference(name, value, Location.Empty)
            {
                Constant = true,
                Bound = true,
            });
        }

        public void DeclareModule(string module, IModule context)
        {
            RemoveModule(module);
            _modules[module] = context;
        }

        public void RemoveModule(string moduleName)
        {
            IModule module;
            if (!_modules.TryGetValue(moduleName, out module))
                return;
            _modules.Remove(moduleName);
        }

        public IModule GetModule(string module)
        {
            IModule context;
            if (_modules.TryGetValue(module, out context))
                return context;
            throw new ScripterRuntimeException($"Module {module} was not declared");
        }

        public void InvalidateModules()
        {
            foreach (var module in _modules)
            {
                module.Value.Invalidate();
            }
        }

        public override GlobalLexicalContext GetGlobalContext() => this;

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
