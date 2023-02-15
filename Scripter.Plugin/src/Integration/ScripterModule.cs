using ScripterLang;

public class ScripterModule : IModule
{
    public string ModuleName => "scripter";

    public ModuleReference Import()
    {
        var module = new ModuleReference();
        module.Exports.Add("self", new ScripterPluginReference());
        return module;
    }

    public void Invalidate()
    {
    }
}
