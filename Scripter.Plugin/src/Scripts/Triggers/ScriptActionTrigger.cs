using System;
using ScripterLang;
using SimpleJSON;

public class ScriptActionTrigger : ScriptTrigger
{
    public const string Type = "Action";

    private readonly JSONStorableAction _action;

    public ScriptActionTrigger(string name, Action<Value> run)
    {
        _action = new JSONStorableAction(name, () => run(Value.Undefined));
        NameJSON = new JSONStorableString("Name", "")
        {
            valNoCallback = name,
            setCallbackFunction = val => _action.name = val
        };
    }

    public static ScriptTrigger FromJSONImpl(JSONNode json, Action<Value> run)
    {
        return new ScriptActionTrigger(
            json["Name"],
            run
        );
    }

    public override JSONNode GetJSON()
    {
        return new JSONClass
        {
            { "Name", _action.name },
            { "Type", Type }
        };
    }

    public override void Register(MVRScript plugin)
    {
        plugin.RegisterAction(_action);
    }

    public override void Deregister(MVRScript plugin)
    {
        plugin.DeregisterAction(_action);
    }
}
