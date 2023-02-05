using System;
using ScripterLang;
using SimpleJSON;

public class ScriptBoolParamTrigger : ScriptTrigger
{
    public const string Type = "BoolParam";

    private readonly JSONStorableBool _valueJSON;

    public override string GetTypeName() => Type;

    public ScriptBoolParamTrigger(string name, Action<Value> run, bool enabled, MVRScript plugin)
        : base(name, enabled, plugin)
    {
        _valueJSON = new JSONStorableBool(name, false, val =>
        {
            if (!EnabledJSON.val) return;
            run(val);
        });
        NameJSON.setCallbackFunction = val =>
        {
            Deregister();
            _valueJSON.name = val;
            Register();
        };
    }

    public static ScriptTrigger FromJSONImpl(JSONNode json, Action<Value> run, MVRScript plugin)
    {
        var trigger = new ScriptBoolParamTrigger(
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
        Plugin.RegisterBool(_valueJSON);
    }

    public override void Deregister()
    {
        Plugin.DeregisterBool(_valueJSON);
    }
}
