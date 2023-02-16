using ScripterLang;

public class ScripterModule : IModule
{
    public string ModuleName => "scripter";

    private static readonly ScripterPluginReference _pluginReference = new ScripterPluginReference();
    private static readonly SceneReference _sceneReference = new SceneReference();
    private static readonly TimeReference _timeReference = new TimeReference();
    private static readonly RandomReference _randomReference = new RandomReference();
    private static readonly DateTimeClassReference _dateTimeClassReference = new DateTimeClassReference();

    private static readonly ModuleNamespace _ns = Initialize();

    private static ModuleNamespace Initialize()
    {
        var module = new ModuleNamespace();
        module.Exports.Add("self", _pluginReference);
        module.Exports.Add("scene", _sceneReference);
        module.Exports.Add("time", _timeReference);
        module.Exports.Add("random", _randomReference);
        module.Exports.Add("datetime", _dateTimeClassReference);
        #warning Call later
        return module;
    }

    public ModuleNamespace Import()
    {
        return _ns;
    }

    public void Invalidate()
    {
    }

    public void Dispose()
    {
    }
}
