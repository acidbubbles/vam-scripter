namespace ScripterLang
{
    public interface IModule
    {
        string ModuleName { get; }
        ModuleReference Import();
        void Invalidate();
    }
}
