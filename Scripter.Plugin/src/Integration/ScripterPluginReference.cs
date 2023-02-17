using ScripterLang;


public class ScripterPluginReference : ObjectReference
{
    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "onUpdate":
                return Func(OnUpdate);
            case "declareFloatParam":
                return Func(DeclareFloatParam);
            case "declareStringParam":
                return Func(DeclareStringParam);
            case "declareBoolParam":
                return Func(DeclareBoolParam);
            case "declareAction":
                return Func(DeclareAction);
            default:
                return base.GetProperty(name);
        }
    }

    private Value OnUpdate(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(OnUpdate), args, 1);
        var fn = args[0].AsFunction;
        var link = new FunctionLink(Scripter.Singleton.OnUpdateFunctions, context, fn);
        context.GetModuleContext().RegisterDisposable(link);
        return Value.Void;
    }

    private Value OnFixedUpdate(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(OnFixedUpdate), args, 1);
        var fn = args[0].AsFunction;
        var link = new FunctionLink(Scripter.Singleton.OnFixedUpdateFunctions, context, fn);
        context.GetModuleContext().RegisterDisposable(link);
        return Value.Void;
    }

    private Value DeclareFloatParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(DeclareFloatParam), args, 1);
        var config = args[0].AsObject;
        var name = config.GetProperty("name").AsString;
        var start = config.GetPropertyWithDefault("default", 0f).AsNumber;
        var min = config.GetPropertyWithDefault("min", 0f).AsNumber;
        var max = config.GetPropertyWithDefault("max", 1f).AsNumber;
        var constrain = config.GetPropertyWithDefault("constrain", true).AsBool;
        var param = new ScripterFloatParamDeclaration(name, start, min, max, constrain);
        var fn = config.GetProperty("onChange");
        context.GetModuleContext().RegisterDisposable(param);
        if (!fn.IsUndefined)
        {
            param.OnChange(context, fn.AsFunction);
        }
        return param;
    }

    private Value DeclareStringParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(DeclareStringParam), args, 1);
        var config = args[0].AsObject;
        var name = config.GetProperty("name").AsString;
        var start = config.GetPropertyWithDefault("default", "").AsString;
        var param = new ScripterStringParamDeclaration(name, start);
        context.GetModuleContext().RegisterDisposable(param);
        var fn = config.GetProperty("onChange");
        context.GetModuleContext().RegisterDisposable(param);
        if (!fn.IsUndefined)
        {
            param.OnChange(context, fn.AsFunction);
        }
        return param;
    }

    private Value DeclareBoolParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(DeclareBoolParam), args, 1);
        var config = args[0].AsObject;
        var name = config.GetProperty("name").AsString;
        var start = config.GetPropertyWithDefault("default", false).AsBool;
        var param = new ScripterBoolParamDeclaration(name, start);
        context.GetModuleContext().RegisterDisposable(param);
        var fn = config.GetProperty("onChange");
        context.GetModuleContext().RegisterDisposable(param);
        if (!fn.IsUndefined)
        {
            param.OnChange(context, fn.AsFunction);
        }
        return param;
    }

    private Value DeclareAction(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(DeclareAction), args, 2);
        var name = args[0].AsString;
        var fn = args[1].AsFunction;
        var param = new ScripterActionDeclaration(name);
        param.OnChange(context, fn);
        context.GetModuleContext().RegisterDisposable(param);
        return param;
    }
}
