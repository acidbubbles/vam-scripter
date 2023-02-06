using System;
using ScripterLang;
using SimpleJSON;

public class ScriptLoadTrigger : ScriptTrigger
{
    public const string Type = "Load";

    public override string GetTypeName() => Type;

    private readonly Action<Value> _run;

    public ScriptLoadTrigger(string name, Action<Value> run, bool enabled, Scripter plugin)
        : base(name, enabled, plugin)
    {
        _run = run;
    }

    public static ScriptTrigger FromJSONImpl(JSONNode json, Action<Value> run, Scripter plugin)
    {
        return new ScriptLoadTrigger(
            json["Name"],
            run,
            json["Enabled"].AsBool,
            plugin
        );
    }

    public void Run()
    {
        if (!EnabledJSON.val) return;
        _run(Value.Void);
    }

    public override void Register()
    {
        Plugin.LoadTriggers.Add(this);
    }

    public override void Deregister()
    {
        Plugin.LoadTriggers.Remove(this);
    }
}
