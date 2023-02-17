using System;
using System.Collections.Generic;

namespace ScripterLang
{
    public class ModuleLexicalContext : FunctionLexicalContext, IDisposable
    {
        public readonly GlobalLexicalContext globalContext;

        public readonly ModuleNamespace module = new ModuleNamespace();
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public ModuleLexicalContext(GlobalLexicalContext globalContext)
            : base(globalContext)
        {
            this.globalContext = globalContext;
        }

        public override ModuleLexicalContext GetModuleContext() => this;

        public override void Exit()
        {
            // TODO: We could clear references to variables neither exported nor used in child contexts, but it wouldn't be worth the effort
        }

        public void RegisterDisposable(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }

            _disposables.Clear();
        }

        public void UnregisterDisposable(IDisposable disposable)
        {
            _disposables.Remove(disposable);
        }
    }
}
