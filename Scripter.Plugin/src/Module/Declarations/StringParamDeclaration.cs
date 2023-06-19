using System;
using System.Globalization;
using ScripterLang;
using SimpleJSON;

public class StringParamDeclaration : ParamDeclarationBase, IDisposable
{
    public const string Type = "StringParam";

    private readonly JSONStorableString _valueJSON;

    public StringParamDeclaration(string name, string startingValue)
    {
        var scripter = Scripter.singleton;
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

    public static ParamDeclarationBase FromJSONImpl(JSONNode json)
    {
        var trigger = new StringParamDeclaration(
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
            case "valNoCallback":
                return _valueJSON.valNoCallback;
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
                _valueJSON.val = value.AsString;
                break;
            case "valNoCallback":
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
        OnChange(context, fn);
        return Value.Void;
    }

    public void OnChange(LexicalContext context, FunctionReference fn)
    {
        _valueJSON.setCallbackFunction = val =>
        {
            try
            {
                _callbackArgs[0] = val;
                fn(context, _callbackArgs);
            }
            catch (Exception e)
            {
                Scripter.singleton.console.LogError($"Exception in {_valueJSON.name} callback: {e.Message}");
            }
        };
    }

    public void Dispose()
    {
        _valueJSON.setCallbackFunction = null;
    }
}
