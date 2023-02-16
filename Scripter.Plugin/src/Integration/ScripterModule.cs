using ScripterLang;

public class ScripterModule : IModule
{
    public string ModuleName => "scripter";


    public ModuleNamespace Import()
    {
        var module = new ModuleNamespace();
        module.Exports.Add("self", new ScripterPluginReference());
        return module;
    }

    public void Invalidate()
    {
    }

    public void Dispose()
    {
    }
}
