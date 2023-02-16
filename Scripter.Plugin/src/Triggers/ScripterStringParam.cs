using System.Globalization;
using ScripterLang;
using SimpleJSON;

public class ScripterStringParam : ScripterParamBase
{
    public const string Type = "StringParam";

    private readonly JSONStorableString _valueJSON;

    public ScripterStringParam(string name, string startingValue)
    {
        var scripter = Scripter.Singleton;
        var existing = scripter.GetStringJSONParam(name);
        if (existing == null)
        {
            _valueJSON = new JSONStorableString(name, startingValue);
            scripter.RegisterString(_valueJSON);
        }
        else
        {
            _valueJSON = existing;
            _valueJSON.defaultVal = startingValue;
        }
    }

    public static ScripterParamBase FromJSONImpl(JSONNode json)
    {
        var trigger = new ScripterStringParam(
            json["Name"],
            json["StartingValue"].Value
        );
        trigger._valueJSON.val = json["Val"].Value;
        return trigger;
    }

    public override JSONClass GetJSON()
    {
        var json = new JSONClass
        {
            { "Type", Type },
            { "Name", _valueJSON.name },
            { "StartingValue", _valueJSON.defaultVal.ToString(CultureInfo.InvariantCulture) },
            { "Val", _valueJSON.val },
        };
        return json;
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "val":
                return _valueJSON.val;
            case "onChange":
                return Func(OnChange);
            default:
                return base.GetProperty(name);
        }
    }

    public override void SetProperty(string name, Value value)
    {
        switch (name)
        {
            case "val":
                _valueJSON.valNoCallback = value.AsString;
                break;
            default:
                base.SetProperty(name, value);
                break;
        }
    }

    private readonly Value[] _callbackArgs = new Value[1];
    private Value OnChange(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(OnChange), args, 1);
        var fn = args[0].AsFunction;
        _valueJSON.setCallbackFunction = val =>
        {
            _callbackArgs[0] = val;
            fn(context, _callbackArgs);
        };
        return Value.Void;
    }
}
