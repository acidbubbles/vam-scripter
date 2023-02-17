using System;
using ScripterLang;
using SimpleJSON;

public class ScripterKeybindingDeclaration : ScripterParamDeclarationBase, IDisposable
{
    private readonly JSONStorableAction _valueJSON;

    public ScripterKeybindingDeclaration(string name)
    {
        var scripter = Scripter.singleton;
        scripter.KeybindingsTriggers.Add(this);
        Scripter.singleton.UpdateKeybindings();
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
        Scripter.singleton.KeybindingsTriggers.Remove(this);
        Scripter.singleton.UpdateKeybindings();
    }
}
