using System;
using System.Collections.Generic;
using ScripterLang;

public class FunctionLink : IDisposable
{
    private readonly List<FunctionLink> _list;
    public readonly LexicalContext context;
    public readonly FunctionReference fn;

    public FunctionLink(List<FunctionLink> list, LexicalContext context, FunctionReference fn)
    {
        _list = list;
        this.context = context;
        this.fn = fn;
        _list.Add(this);
    }

    public void Dispose()
    {
        _list.Remove(this);
    }
}
