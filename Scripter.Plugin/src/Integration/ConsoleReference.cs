using ScripterLang;

public class ConsoleReference : ObjectReference
{
    public override Value Get(string name)
    {
        switch (name)
        {
            case "clear":
                return Func(Clear);
            case "log":
                return Func(Log);
            case "error":
                return Func(Error);
            default:
                return base.Get(name);
        }
    }

    private static Value Clear(LexicalContext context, Value[] args)
    {
        Scripter.Singleton.Scripts.ConsoleJSON.val = "";
        return Value.Void;
    }

    private static Value Log(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(Log), args, 1);
        Scripter.Singleton.Scripts.Log(args[0].Stringify);
        return Value.Void;
    }

    private static Value Error(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(Error), args, 1);
        Scripter.Singleton.Scripts.LogError(args[0].Stringify);
        return Value.Void;
    }
}
