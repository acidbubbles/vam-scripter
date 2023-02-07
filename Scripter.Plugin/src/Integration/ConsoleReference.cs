using ScripterLang;

public class ConsoleReference : Reference
{
    public override Value InvokeMethod(string name, Value[] args)
    {
        switch (name)
        {
            case "clear":
                SuperController.singleton.ClearMessages();
                return Value.Void;
            case "log":
                ValidateArgumentsLength(name, args, 1);
                SuperController.LogMessage(args[0].Stringify);
                return Value.Void;
            case "error":
                ValidateArgumentsLength(name, args, 1);
                SuperController.LogError(args[0].Stringify);
                return Value.Void;
            default:
                return base.InvokeMethod(name, args);
        }
    }
}
