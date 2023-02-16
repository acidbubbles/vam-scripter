using System;

namespace ScripterLang
{
    public interface IModule : IDisposable
    {
        string ModuleName { get; }
        ModuleNamespace Import();
        void Invalidate();
    }
}
