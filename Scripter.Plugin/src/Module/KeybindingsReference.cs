using ScripterLang;
using UnityEngine;

public class KeybindingsReference : ObjectReference
{
    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "invokeCommand":
                return Func(InvokeCommand);
            case "declareCommand":
                return Func(DeclareCommand);
            default:
                return base.GetProperty(name);
        }
    }

    private Value InvokeCommand(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(InvokeCommand), args, 1);
        var commandName = args[0].AsString;
        SuperController.singleton.BroadcastMessage("InvokeKeybindingsAction", commandName, SendMessageOptions.DontRequireReceiver);
        return Value.Void;
    }

    private Value DeclareCommand(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(DeclareCommand), args, 2);
        var name = args[0].AsString;
        var fn = args[1].AsFunction;
        var param = new KeybindingDeclaration(name);
        context.GetModuleContext().RegisterDisposable(param);
        param.OnTrigger(context, fn);
        return param;
    }
}
