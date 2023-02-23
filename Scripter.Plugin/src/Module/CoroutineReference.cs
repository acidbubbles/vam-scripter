using System;
using System.Collections;
using ScripterLang;
using UnityEngine;

public class CoroutineIterator : ObjectReference
{
    public const int Break = -1;
    public const int NextFrame = 0;
    public const int WaitForSeconds = 2;

    public int result = -1;
    public float waitForSecondsValue;
    private readonly Value _waitForSeconds;

    public CoroutineIterator()
    {
        _waitForSeconds = Func(WaitForSecondsImpl);
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "stop":
                return result = Break;
            case "nextFrame":
                return result = NextFrame;
            case "waitForSeconds":
                return _waitForSeconds;
            default:
                return base.GetProperty(name);
        }
    }

    private Value WaitForSecondsImpl(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(WaitForSeconds), args, 1);
        result = WaitForSeconds;
        waitForSecondsValue = args[0].AsNumber;
        if(waitForSecondsValue < 0) throw new ScripterRuntimeException("WaitForSeconds value must be greater than 0");
        return WaitForSeconds;
    }
}

public class CoroutineReference : ObjectReference, IDisposable
{
    private readonly LexicalContext _context;
    private readonly FunctionReference _fn;
    private readonly CoroutineIterator _iterator;
    private readonly Value[] _args;

    private Coroutine _co;

    public CoroutineReference(LexicalContext context, FunctionReference fn, CoroutineIterator iterator)
    {
        _context = context;
        _fn = fn;
        _iterator = iterator;
        _args = new Value[] { _iterator };
    }

    public void Start()
    {
        _co = Scripter.singleton.StartCoroutine(Coroutine());
    }

    private IEnumerator Coroutine()
    {
        while (true)
        {
            _fn(_context, _args);
            switch (_iterator.result)
            {
                case CoroutineIterator.Break:
                    _context.GetModuleContext().UnregisterDisposable(this);
                    yield break;
                case CoroutineIterator.NextFrame:
                    yield return 0;
                    continue;
                case CoroutineIterator.WaitForSeconds:
                    yield return new WaitForSeconds(_iterator.waitForSecondsValue);
                    continue;
                default:
                    throw new NotSupportedException($"Unexpected iterator result {_iterator.result}");
            }
        }
    }

    public void Dispose()
    {
        if (_co != null)
            Scripter.singleton.StopCoroutine(_co);
    }
}
