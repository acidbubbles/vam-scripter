using System.Globalization;
using ScripterLang;

public static class Globals
{
    public static void Register(GlobalLexicalContext lexicalContext)
    {
        lexicalContext.DeclareGlobal("console", new ConsoleReference());
        lexicalContext.DeclareGlobal("Math", new MathReference());
        lexicalContext.DeclareGlobal("JSON", new JSON());
        lexicalContext.DeclareGlobal("setTimeout", Value.CreateFunction(SetTimeout));
        lexicalContext.DeclareGlobal("clearTimeout", Value.CreateFunction(ClearTimeout));
        lexicalContext.DeclareGlobal("parseInt", Value.CreateFunction(ParseInt));
        lexicalContext.DeclareGlobal("parseFloat", Value.CreateFunction(ParseFloat));
        lexicalContext.DeclareGlobal("isNaN", Value.CreateFunction(IsNaN));
        lexicalContext.DeclareModule(new ScripterModule());
    }

    private static Value SetTimeout(LexicalContext context, Value[] args)
    {
        if(args.Length != 2)
            throw new ScripterRuntimeException($"{nameof(SetTimeout)} requires 2 arguments");
        var fn = args[0].AsFunction;
        var delay = args[1].AsNumber;
        var coRef = new SetTimeoutToken(context, fn, delay);
        context.GetModuleContext().RegisterDisposable(coRef);
        coRef.Start();
        return coRef;
    }

    private static Value ClearTimeout(LexicalContext context, Value[] args)
    {
        if(args.Length != 1)
            throw new ScripterRuntimeException($"{nameof(ClearTimeout)} requires 1 argument");
        var coRef = args[0].AsObject as SetTimeoutToken;
        if(coRef == null)
            throw new ScripterRuntimeException($"{nameof(ClearTimeout)} requires a SetTimeoutToken as argument");
        coRef.Dispose();
        context.GetModuleContext().UnregisterDisposable(coRef);
        return Value.Void;
    }

    private static Value ParseInt(LexicalContext context, Value[] args)
    {
        if(args.Length != 1)
            throw new ScripterRuntimeException($"{nameof(ParseInt)} requires 1 argument");
        if (args[0].IsUndefined) return float.NaN;
        var value = args[0].AsString;
        return int.Parse(value, CultureInfo.InvariantCulture);
    }

    private static Value ParseFloat(LexicalContext context, Value[] args)
    {
        if(args.Length != 1)
            throw new ScripterRuntimeException($"{nameof(ParseInt)} requires 1 argument");
        if (args[0].IsUndefined) return float.NaN;
        var value = args[0].AsString;
        return float.Parse(value, CultureInfo.InvariantCulture);
    }

    private static Value IsNaN(LexicalContext context, Value[] args)
    {
        if(args.Length != 1)
            throw new ScripterRuntimeException($"{nameof(ParseInt)} requires 1 argument");
        return float.IsNaN(args[0].AsFloat);
    }
}
