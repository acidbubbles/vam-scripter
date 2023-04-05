using System;
using System.Collections.Generic;
using ScripterLang;

public class FunctionLink : IDisposable
{
    private readonly List<FunctionLink> _list;
    public readonly LexicalContext context;
    public readonly FunctionReference fn;
    public readonly string name;

    public FunctionLink(List<FunctionLink> list, LexicalContext context, FunctionReference fn, string name)
    {
        _list = list;
        this.context = context;
        this.fn = fn;
        this.name = name;
        _list.Add(this);
    }

    public void Dispose()
    {
        _list.Remove(this);
    }
}
