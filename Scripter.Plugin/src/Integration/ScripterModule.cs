using ScripterLang;

public class ScripterModule : IModule
{
    public string ModuleName => "scripter";

    private static readonly ScripterPluginReference _pluginReference = new ScripterPluginReference();
    private static readonly SceneReference _sceneReference = new SceneReference();
    private static readonly TimeReference _timeReference = new TimeReference();
    private static readonly RandomReference _randomReference = new RandomReference();
    private static readonly DateTimeClassReference _dateTimeClassReference = new DateTimeClassReference();
    private static readonly EnvironmentReference _environmentReference = new EnvironmentReference();
    private static readonly KeybindingsReference _keybindingsReference = new KeybindingsReference();

    private static readonly ModuleNamespace _ns = Initialize();

    private static ModuleNamespace Initialize()
    {
        var module = new ModuleNamespace();
        module.exports.Add("self", _pluginReference);
        module.exports.Add("scene", _sceneReference);
        module.exports.Add("time", _timeReference);
        module.exports.Add("random", _randomReference);
        module.exports.Add("datetime", _dateTimeClassReference);
        module.exports.Add("environment", _environmentReference);
        module.exports.Add("keybindings", _keybindingsReference);
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
