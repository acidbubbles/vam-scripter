using System;
using ScripterLang;
using SimpleJSON;

public class KeybindingDeclaration : ParamDeclarationBase, IDisposable
{
    public readonly JSONStorableAction actionJSON;

    public KeybindingDeclaration(string name)
    {
        actionJSON = new JSONStorableAction(name, null);
        var scripter = Scripter.singleton;
        scripter.keybindingsTriggers.Add(this);
        Scripter.singleton.UpdateKeybindings();
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
        actionJSON.actionCallback = () =>
        {
            try
            {
                fn(context, Value.EmptyValues);
            }
            catch (Exception e)
            {
                Scripter.singleton.console.LogError($"Exception in keybindings {actionJSON.name} callback: {e.Message}");
            }
        };
    }

    public void Dispose()
    {
        actionJSON.actionCallback = null;
        Scripter.singleton.keybindingsTriggers.Remove(this);
        Scripter.singleton.UpdateKeybindings();
    }
}
