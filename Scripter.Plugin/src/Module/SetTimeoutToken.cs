using System;
using System.Collections;
using ScripterLang;
using UnityEngine;

internal class SetTimeoutToken : ObjectReference, IDisposable
{
    private readonly LexicalContext _context;
    private readonly FunctionReference _fn;
    private readonly float _delay;

    private Coroutine _co;

    public SetTimeoutToken(LexicalContext context, FunctionReference fn, float delay)
    {
        _context = context;
        _fn = fn;
        _delay = delay;
    }

    public void Start()
    {
        _co = Scripter.singleton.StartCoroutine(RunLater(_context, _fn, _delay));
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
            Scripter.singleton.StopCoroutine(_co);
    }
}
