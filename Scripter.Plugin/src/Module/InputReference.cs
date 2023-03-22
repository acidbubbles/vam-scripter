using System;
using ScripterLang;
using UnityEngine;

public class InputReference : ObjectReference
{
    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "onKeyDown":
                return Func(OnKeyDown);
            case "onKeyUp":
                return Func(OnKeyUp);
            default:
                return base.GetProperty(name);
        }
    }

    private Value OnKeyDown(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(OnKeyDown), args, 2);
        var keyName = args[0].AsString;
        var key = (KeyCode)Enum.Parse(typeof(KeyCode), keyName);
        var fn = args[1].AsFunction;
        var link = new FunctionLink(Scripter.singleton.onUpdateFunctions, context, (lexicalContext, values) =>
        {
            if (!Input.GetKeyDown(key)) return Value.Void;
            fn(lexicalContext, Value.EmptyValues);
            return Value.Void;
        });
        context.GetModuleContext().RegisterDisposable(link);
        return Value.Void;
    }

    private Value OnKeyUp(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(OnKeyUp), args, 2);
        var keyName = args[0].AsString;
        var key = (KeyCode)Enum.Parse(typeof(KeyCode), keyName);
        var fn = args[1].AsFunction;
        var link = new FunctionLink(Scripter.singleton.onUpdateFunctions, context, (lexicalContext, values) =>
        {
            if (!Input.GetKeyUp(key)) return Value.Void;
            fn(lexicalContext, Value.EmptyValues);
            return Value.Void;
        });
        context.GetModuleContext().RegisterDisposable(link);
        return Value.Void;
    }
}
