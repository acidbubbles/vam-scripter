using System;
using ScripterLang;
using SimpleJSON;

public abstract class ScriptTrigger
{
    public static ScriptTrigger Create(string type, string name, Action<Value> run)
    {
        switch (type)
        {
            case ScriptActionTrigger.Type:
                return new ScriptActionTrigger(name, run);
            default:
                throw new NotSupportedException($"Trigger type {type} is not supported. Maybe you're running an old version of Scripter?");
        }
    }

    public static ScriptTrigger FromJSON(JSONNode json, Action<Value> run)
    {
        switch (json["Type"].Value)
        {
            case ScriptActionTrigger.Type:
                return ScriptActionTrigger.FromJSONImpl(json, run);
            default:
                throw new NotSupportedException($"Trigger type {json["Type"].Value} is not supported. Maybe you're running an old version of Scripter?");
        }
    }

    public JSONStorableString NameJSON;

    public abstract JSONNode GetJSON();
    public abstract void Register(MVRScript plugin);
    public abstract void Deregister(MVRScript plugin);
}
