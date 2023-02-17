using System;
using ScripterLang;
using SimpleJSON;

public class ScripterKeybindingDeclaration : ScripterParamDeclarationBase, IDisposable
{
    public const string Type = "Action";

    private readonly JSONStorableAction _valueJSON;

    public ScripterKeybindingDeclaration(string name)
    {
        var scripter = Scripter.Singleton;
        scripter.KeybindingsTriggers.Add(this);
        Scripter.Singleton.UpdateKeybindings();
        _valueJSON = new JSONStorableAction(name, () => { });
    }

    public override JSONClass GetJSON()
    {
        throw new NotSupportedException("No JSON serialization for keybindings.");
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
        Scripter.Singleton.KeybindingsTriggers.Remove(this);
        Scripter.Singleton.UpdateKeybindings();
    }
}
