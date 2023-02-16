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

        public void DeclareModule(IModule module)
        {
            RemoveModule(module.ModuleName);
            SuperController.LogMessage("Registering " + module.ModuleName + " from source");
            _modules[module.ModuleName] = module;
        }

        public void RemoveModule(string moduleName)
        {
            SuperController.LogMessage("Removing " + moduleName + " from global context");
            IModule module;
            if (!_modules.TryGetValue(moduleName, out module))
                return;
            SuperController.LogMessage("Disposing " + moduleName + " from global context");
            module.Dispose();
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

        public override LoopLexicalContext GetLoopContext()
        {
            return null;
        }

        public override FunctionLexicalContext GetFunctionContext()
        {
            return null;
        }
    }
}
