using System;
using System.Collections.Generic;
using ScripterLang;

public class FunctionLink : IDisposable
{
    private readonly List<FunctionLink> _list;
    public readonly LexicalContext Context;
    public readonly FunctionReference Fn;

    public FunctionLink(List<FunctionLink> list, LexicalContext context, FunctionReference fn)
    {
        _list = list;
        Context = context;
        Fn = fn;
        _list.Add(this);
    }

    public void Dispose()
    {
        _list.Remove(this);
    }
}
