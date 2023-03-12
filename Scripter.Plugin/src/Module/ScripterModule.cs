using ScripterLang;

public class ScripterModule : IModule
{
    public string ModuleName => "vam-scripter";

    private static readonly ScripterPluginReference _pluginReference = new ScripterPluginReference();
    private static readonly SceneReference _sceneReference = new SceneReference();
    private static readonly TimeReference _timeReference = new TimeReference();
    private static readonly RandomReference _randomReference = new RandomReference();
    private static readonly DateTimeClassReference _dateTimeClassReference = new DateTimeClassReference();
    private static readonly PlayerReference _playerReference = new PlayerReference();
    private static readonly KeybindingsReference _keybindingsReference = new KeybindingsReference();

    private static readonly ModuleNamespace _ns = Initialize();

    private static ModuleNamespace Initialize()
    {
        var module = new ModuleNamespace();
        module.exports.Add("scripter", _pluginReference);
        module.exports.Add("scene", _sceneReference);
        module.exports.Add("Time", _timeReference);
        module.exports.Add("Random", _randomReference);
        module.exports.Add("DateTime", _dateTimeClassReference);
        module.exports.Add("player", _playerReference);
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
