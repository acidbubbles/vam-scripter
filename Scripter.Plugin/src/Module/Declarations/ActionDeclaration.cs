using System;
using ScripterLang;
using SimpleJSON;

public class ActionDeclaration : ParamDeclarationBase, IDisposable
{
    public const string Type = "Action";

    private readonly JSONStorableAction _valueJSON;

    public ActionDeclaration(string name)
    {
        var scripter = Scripter.singleton;
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

    public static ParamDeclarationBase FromJSONImpl(JSONNode json)
    {
        var trigger = new ActionDeclaration(
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
                return Func(OnTrigger);
            default:
                return base.GetProperty(name);
        }
    }

    private Value OnTrigger(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(OnTrigger), args, 1);
        var fn = args[0].AsFunction;
        OnTrigger(context, fn);
        return Value.Void;
    }

    public void OnTrigger(LexicalContext context, FunctionReference fn)
    {
        _valueJSON.actionCallback = () => { fn(context, Value.EmptyValues); };
    }

    public void Dispose()
    {
        _valueJSON.actionCallback = null;
    }
}
