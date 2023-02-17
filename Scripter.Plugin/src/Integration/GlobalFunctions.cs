using System;
using System.Collections;
using ScripterLang;
using UnityEngine;

public static class GlobalFunctions
{
    public static void Register(GlobalLexicalContext lexicalContext)
    {
        lexicalContext.DeclareGlobal("console", new ConsoleReference());
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
        var coRef = new CoroutineReference(context, fn, delay);
        context.GetModuleContext().RegisterDisposable(coRef);
        coRef.Start();
        return coRef;
    }

    private static Value ClearTimeout(LexicalContext context, Value[] args)
    {
        if(args.Length != 1)
            throw new ScripterRuntimeException($"{nameof(ClearTimeout)} requires 1 argument");
        var coRef = args[0].AsObject as CoroutineReference;
        if(coRef == null)
            throw new ScripterRuntimeException($"{nameof(ClearTimeout)} requires a CoroutineReference as argument");
        coRef.Dispose();
        context.GetModuleContext().UnregisterDisposable(coRef);
        return Value.Void;
    }
}

internal class CoroutineReference : ObjectReference, IDisposable
{
    private readonly LexicalContext _context;
    private readonly FunctionReference _fn;
    private readonly float _delay;

    private Coroutine _co;

    public CoroutineReference(LexicalContext context, FunctionReference fn, float delay)
    {
        _context = context;
        _fn = fn;
        _delay = delay;
    }

    public void Start()
    {
        _co = Scripter.Singleton.StartCoroutine(RunLater(_context, _fn, _delay));
    }

    private IEnumerator RunLater(LexicalContext context, FunctionReference fn, float delay)
    {
        if (delay == 0)
            yield return 0;
        else
            yield return new WaitForSeconds(delay);
        fn(context, Value.EmptyValues);
        context.GetModuleContext().UnregisterDisposable(this);
    }

    public void Dispose()
    {
        if (_co != null)
            Scripter.Singleton.StopCoroutine(_co);
    }
}
