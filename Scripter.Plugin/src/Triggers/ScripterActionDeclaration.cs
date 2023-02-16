using System;
using ScripterLang;
using SimpleJSON;

public class ScripterActionDeclaration : ScripterParamDeclarationBase, IDisposable
{
    public const string Type = "Action";

    private readonly JSONStorableAction _valueJSON;

    public ScripterActionDeclaration(string name)
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

    public static ScripterParamDeclarationBase FromJSONImpl(JSONNode json)
    {
        var trigger = new ScripterActionDeclaration(
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

    public void Dispose()
    {
        _valueJSON.actionCallback = null;
    }
}
