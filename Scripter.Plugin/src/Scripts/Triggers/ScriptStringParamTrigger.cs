using System;
using ScripterLang;
using SimpleJSON;

public class ScriptStringParamTrigger : ScriptTrigger
{
    public const string Type = "StringParam";

    private readonly JSONStorableString _valueJSON;

    public override string GetTypeName() => Type;

    public ScriptStringParamTrigger(string name, Action<Value> run, bool enabled, Scripter plugin)
        : base(name, enabled, plugin)
    {
        _valueJSON = new JSONStorableString(name, "", val =>
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

    public static ScriptTrigger FromJSONImpl(JSONNode json, Action<Value> run, Scripter plugin)
    {
        var trigger = new ScriptStringParamTrigger(
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
        Plugin.RegisterString(_valueJSON);
    }

    public override void Deregister()
    {
        Plugin.DeregisterString(_valueJSON);
    }
}
