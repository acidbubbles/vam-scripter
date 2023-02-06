using System;
using ScripterLang;
using SimpleJSON;

public class ScriptKeybindingsTrigger : ScriptTrigger
{
    public const string Type = "Keybindings";

    public override string GetTypeName() => Type;

    private readonly Action<Value> _run;

    public ScriptKeybindingsTrigger(string name, Action<Value> run, bool enabled, Scripter plugin)
        : base(name, enabled, plugin)
    {
        _run = run;
        NameJSON.setCallbackFunction = val => plugin.UpdateKeybindings();
    }

    public static ScriptTrigger FromJSONImpl(JSONNode json, Action<Value> run, Scripter plugin)
    {
        return new ScriptKeybindingsTrigger(
            json["Name"],
            run,
            json["Enabled"].AsBool,
            plugin
        );
    }

    public void Run()
    {
        if (!EnabledJSON.val) return;
        _run(Value.Undefined);
    }

    public override void Register()
    {
        Plugin.KeybindingsTriggers.Add(this);
        Plugin.UpdateKeybindings();
    }

    public override void Deregister()
    {
        Plugin.KeybindingsTriggers.Remove(this);
        Plugin.UpdateKeybindings();
    }
}
