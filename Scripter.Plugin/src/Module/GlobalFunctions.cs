using ScripterLang;


public static class GlobalFunctions
{
    public static void Register(GlobalLexicalContext lexicalContext)
    {
        lexicalContext.DeclareGlobal("console", new ConsoleReference());
        lexicalContext.DeclareGlobal("Math", new MathReference());
        lexicalContext.DeclareGlobal("setTimeout", Value.CreateFunction(SetTimeout));
        lexicalContext.DeclareGlobal("clearTimeout", Value.CreateFunction(ClearTimeout));
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
            throw new ScripterRuntimeException($"{nameof(ClearTimeout)} requires a CoroutineReference as argument");
        coRef.Dispose();
        context.GetModuleContext().UnregisterDisposable(coRef);
        return Value.Void;
    }
}
