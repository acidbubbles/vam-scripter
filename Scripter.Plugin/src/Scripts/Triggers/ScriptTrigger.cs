using System;
using ScripterLang;
using SimpleJSON;

public abstract class ScriptTrigger
{
    public static ScriptTrigger Create(string type, string name, Action<Value> run, MVRScript plugin)
    {
        switch (type)
        {
            case ScriptActionTrigger.Type:
                return new ScriptActionTrigger(name, run, true, plugin);
            case ScriptFloatParamTrigger.Type:
                return new ScriptFloatParamTrigger(name, run, true, plugin);
            case ScriptBoolParamTrigger.Type:
                return new ScriptBoolParamTrigger(name, run, true, plugin);
            case ScriptStringParamTrigger.Type:
                return new ScriptStringParamTrigger(name, run, true, plugin);
            case ScriptUpdateTrigger.Type:
                return new ScriptUpdateTrigger(name, run, false, plugin);
            default:
                throw new NotSupportedException($"Trigger type {type} is not supported. Maybe you're running an old version of Scripter?");
        }
    }

    public static ScriptTrigger FromJSON(JSONNode json, Action<Value> run, MVRScript plugin)
    {
        switch (json["Type"].Value)
        {
            case ScriptActionTrigger.Type:
                return ScriptActionTrigger.FromJSONImpl(json, run, plugin);
            case ScriptFloatParamTrigger.Type:
                return ScriptFloatParamTrigger.FromJSONImpl(json, run, plugin);
            case ScriptBoolParamTrigger.Type:
                return ScriptBoolParamTrigger.FromJSONImpl(json, run, plugin);
            case ScriptStringParamTrigger.Type:
                return ScriptStringParamTrigger.FromJSONImpl(json, run, plugin);
            case ScriptUpdateTrigger.Type:
                return ScriptUpdateTrigger.FromJSONImpl(json, run, plugin);
            default:
                throw new NotSupportedException($"Trigger type {json["Type"].Value} is not supported. Maybe you're running an old version of Scripter?");
        }
    }

    protected readonly MVRScript Plugin;

    public readonly JSONStorableString NameJSON;
    public readonly JSONStorableBool EnabledJSON;

    protected ScriptTrigger(string name, bool enabled, MVRScript plugin)
    {
        Plugin = plugin;
        NameJSON = new JSONStorableString("Name", "")
        {
            valNoCallback = name,
        };
        EnabledJSON = new JSONStorableBool("Enabled", enabled);
    }

    public abstract string GetTypeName();
    public abstract void Register();
    public abstract void Deregister();

    public virtual JSONClass GetJSON()
    {
        return new JSONClass
        {
            { "Name", NameJSON.val },
            { "Type", GetTypeName() },
            { "Enabled", EnabledJSON.val ? "true" : "false" }
        };
    }
}
