using System.Linq;
using ScripterLang;

public class ConsoleReference : ObjectReference
{
    private readonly ConsoleBuffer _console = Scripter.singleton.console;
    private readonly Value _clear;
    private readonly Value _log;
    private readonly Value _error;

    public ConsoleReference()
    {
        _clear = Func(Clear);
        _log = Func(Log);
        _error = Func(Error);
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "clear":
                return _clear;
            case "log":
                return _log;
            case "error":
                return _error;
            default:
                return base.GetProperty(name);
        }
    }

    private Value Clear(LexicalContext context, Value[] args)
    {
        _console.Clear();
        return Value.Void;
    }

    private Value Log(LexicalContext context, Value[] args)
    {
        if (args.Length == 0)
            return Value.Void;
        if (args.Length == 1)
        {
            _console.Log(args[0].Stringify);
            return Value.Void;
        }

        //TODO: All .Select().ToArray() should reuse a single string join util that has pre-created arrays
        _console.Log(string.Join(" ", args.Select(x => x.Stringify).ToArray()));
        return Value.Void;
    }

    private Value Error(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(Error), args, 1);
        _console.LogError(args[0].Stringify);
        return Value.Void;
    }
}
