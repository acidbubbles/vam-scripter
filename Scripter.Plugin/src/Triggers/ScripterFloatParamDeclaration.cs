using System;
using System.Globalization;
using ScripterLang;
using SimpleJSON;

public class ScripterFloatParamDeclaration : ScripterParamDeclarationBase, IDisposable
{
    public const string Type = "FloatParam";

    private readonly JSONStorableFloat _valueJSON;

    public ScripterFloatParamDeclaration(string name, float startingValue, float minValue, float maxValue, bool constrain)
    {
        var scripter = Scripter.singleton;
        var existing = scripter.GetFloatJSONParam(name);
        if (existing == null)
        {
            _valueJSON = new JSONStorableFloat(name, startingValue, minValue, maxValue, constrain);
            scripter.RegisterFloat(_valueJSON);
        }
        else
        {
            _valueJSON = existing;
            _valueJSON.defaultVal = startingValue;
            _valueJSON.min = minValue;
            _valueJSON.max = maxValue;
            _valueJSON.constrained = constrain;
        }
    }

    public static ScripterParamDeclarationBase FromJSONImpl(JSONNode json)
    {
        var trigger = new ScripterFloatParamDeclaration(
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
            { "Type", Type },
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

    private readonly Value[] _callbackArgs = new Value[1];
    private Value OnChange(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(OnChange), args, 1);
        var fn = args[0].AsFunction;
        OnChange(context, fn);
        return Value.Void;
    }

    public void OnChange(LexicalContext context, FunctionReference fn)
    {
        _valueJSON.setCallbackFunction = val =>
        {
            _callbackArgs[0] = val;
            fn(context, _callbackArgs);
        };
    }

    public void Dispose()
    {
        _valueJSON.setCallbackFunction = null;
    }
}
