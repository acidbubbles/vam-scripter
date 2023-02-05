using System;
using ScripterLang;
using SimpleJSON;

public class ScriptFloatParamTrigger : ScriptTrigger
{
    public const string Type = "FloatParam";

    private readonly JSONStorableFloat _valueJSON;

    public override string GetTypeName() => Type;

    public ScriptFloatParamTrigger(string name, Action<Value> run, bool enabled, MVRScript plugin)
        : base(name, enabled, plugin)
    {
        _valueJSON = new JSONStorableFloat(name, 0, val => run(val), 0, 1, false);
        NameJSON.setCallbackFunction = val =>
        {
            Deregister();
            _valueJSON.name = val;
            Register();
        };
    }

    public static ScriptTrigger FromJSONImpl(JSONNode json, Action<Value> run, MVRScript plugin)
    {
        var trigger = new ScriptFloatParamTrigger(
            json["Name"],
            run,
            json["Enabled"].AsBool,
            plugin
        );
        trigger._valueJSON.RestoreFromJSON(json.AsObject);
        return trigger;
    }

    public override JSONClass GetJSON()
    {
        var json = base.GetJSON();
        _valueJSON.StoreJSON(json);
        return json;
    }

    public override void Register()
    {
        Plugin.RegisterFloat(_valueJSON);
    }

    public override void Deregister()
    {
        Plugin.DeregisterFloat(_valueJSON);
    }
}
