using System.Globalization;
using ScripterLang;
using SimpleJSON;

public class ScripterFloatParam : ScripterParamBase
{
    public const string Type = "FloatParam";

    private readonly JSONStorableFloat _valueJSON;

    public ScripterFloatParam(string name, float startingValue, float minValue, float axValue, bool constrain)
    {
        _valueJSON = new JSONStorableFloat(name, startingValue, minValue, axValue, constrain);
        #warning Unregister? Check if already exists?
        Scripter.Singleton.RegisterFloat(_valueJSON);
    }

    public static ScripterParamBase FromJSONImpl(JSONNode json)
    {
        var trigger = new ScripterFloatParam(
            json["Name"],
            json["StartingValue"].AsFloat,
            json["MinValue"].AsFloat,
            json["MaxValue"].AsFloat,
            json["Constrain"].AsBool
        );
        trigger._valueJSON.val = json["Val"].AsFloat;
        return trigger;
    }

    public override JSONClass GetJSON()
    {
        var json = new JSONClass
        {
            { "Name", _valueJSON.name },
            { "StartingValue", _valueJSON.defaultVal.ToString(CultureInfo.InvariantCulture) },
            { "MinValue", _valueJSON.min.ToString(CultureInfo.InvariantCulture) },
            { "MaxValue", _valueJSON.max.ToString(CultureInfo.InvariantCulture) },
            { "Constrain", _valueJSON.constrained.ToString() },
            { "Val", _valueJSON.val.ToString(CultureInfo.InvariantCulture) },
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
                _valueJSON.valNoCallback = value.AsNumber;
                break;
            default:
                base.SetProperty(name, value);
                break;
        }
    }

    private Value OnChange(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(OnChange), args, 1);
        var fn = args[0].AsFunction;
        #warning Check if we can add more of those to avoid allocations in function calls whenever possible
        var callbackArgs = new Value[1];
        _valueJSON.setCallbackFunction = val =>
        {
            callbackArgs[0] = val;
            fn(context, callbackArgs);
        };
        return Value.Void;
    }
}
