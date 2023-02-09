using ScripterLang;

public class ConsoleReference : ObjectReference
{
    public override Value Get(string name)
    {
        switch (name)
        {
            case "clear":
                return fn(Clear);
            case "log":
                return fn(Log);
            case "error":
                return fn(Error);
            default:
                return base.Get(name);
        }
    }

    private static Value Clear(LexicalContext context, Value[] args)
    {
        SuperController.singleton.ClearMessages();
        return Value.Void;
    }

    private static Value Log(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(Log), args, 1);
        SuperController.LogMessage(args[0].Stringify);
        return Value.Void;
    }

    private static Value Error(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(Error), args, 1);
        SuperController.LogError(args[0].Stringify);
        return Value.Void;
    }
}
