using System;
using ScripterLang;
using SimpleJSON;

public class ScriptActionTrigger : ScriptTrigger
{
    public const string Type = "Action";

    private readonly JSONStorableAction _actionJSON;

    public override string GetTypeName() => Type;

    public ScriptActionTrigger(string name, Action<Value> run, bool enabled, MVRScript plugin)
        : base(name, enabled, plugin)
    {
        _actionJSON = new JSONStorableAction(name, () => run(Value.Undefined));
        NameJSON.setCallbackFunction = val =>
        {
            Deregister();
            _actionJSON.name = val;
            Register();
        };
    }

    public static ScriptTrigger FromJSONImpl(JSONNode json, Action<Value> run, MVRScript plugin)
    {
        return new ScriptActionTrigger(
            json["Name"],
            run,
            json["Enabled"].AsBool,
            plugin
        );
    }

    public override void Register()
    {
        Plugin.RegisterAction(_actionJSON);
    }

    public override void Deregister()
    {
        Plugin.DeregisterAction(_actionJSON);
    }
}
