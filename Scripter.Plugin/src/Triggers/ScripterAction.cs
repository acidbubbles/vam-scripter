using ScripterLang;
using SimpleJSON;

public class ScripterAction : ScripterParamBase
{
    public const string Type = "Action";

    private readonly JSONStorableAction _valueJSON;

    public ScripterAction(string name)
    {
        var scripter = Scripter.Singleton;
        var existing = scripter.GetAction(name);
        if (existing == null)
        {
            _valueJSON = new JSONStorableAction(name, () => { });
            scripter.RegisterAction(_valueJSON);
        }
        else
        {
            _valueJSON = existing;
            _valueJSON.actionCallback = () => { };
        }
    }

    public static ScripterParamBase FromJSONImpl(JSONNode json)
    {
        var trigger = new ScripterAction(
            json["Name"]
        );
        return trigger;
    }

    public override JSONClass GetJSON()
    {
        var json = new JSONClass
        {
            { "Type", Type },
            { "Name", _valueJSON.name },
        };
        return json;
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "onTrigger":
                return Func(OnChange);
            default:
                return base.GetProperty(name);
        }
    }


    private readonly Value[] _callbackArgs = new Value[0];
    private Value OnChange(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(OnChange), args, 1);
        var fn = args[0].AsFunction;
        _valueJSON.actionCallback = () =>
        {
            fn(context, _callbackArgs);
        };
        return Value.Void;
    }
}
