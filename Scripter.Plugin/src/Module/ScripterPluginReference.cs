using ScripterLang;


public class ScripterPluginReference : ObjectReference
{
    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "onUpdate":
                return Func(OnUpdate);
            case "onFixedUpdate":
                return Func(OnFixedUpdate);
            case "startCoroutine":
                return Func(StartCoroutine);
            case "stopCoroutine":
                return Func(StopCoroutine);
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
        var link = new FunctionLink(Scripter.singleton.onUpdateFunctions, context, fn);
        context.GetModuleContext().RegisterDisposable(link);
        return Value.Void;
    }

    private Value OnFixedUpdate(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(OnFixedUpdate), args, 1);
        var fn = args[0].AsFunction;
        var link = new FunctionLink(Scripter.singleton.onFixedUpdateFunctions, context, fn);
        context.GetModuleContext().RegisterDisposable(link);
        return Value.Void;
    }

    private Value StartCoroutine(LexicalContext context, Value[] args)
    {
        // TODO: Instead use *function and yield?
        ValidateArgumentsLength(nameof(StartCoroutine), args, 1);
        var fn = args[0].AsFunction;
        var coRef = new CoroutineReference(context, fn, new CoroutineIterator());
        context.GetModuleContext().RegisterDisposable(coRef);
        coRef.Start();
        return coRef;
    }

    private Value StopCoroutine(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(StopCoroutine), args, 1);
        var coRef = args[0].AsObject as CoroutineReference;
        if(coRef == null)
            throw new ScripterRuntimeException($"{nameof(StopCoroutine)} requires a CoroutineReference as argument");
        coRef.Dispose();
        context.GetModuleContext().UnregisterDisposable(coRef);
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
        var param = new FloatParamDeclaration(name, start, min, max, constrain);
        context.GetModuleContext().RegisterDisposable(param);
        var fn = config.GetProperty("onChange");
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
        var param = new StringParamDeclaration(name, start);
        context.GetModuleContext().RegisterDisposable(param);
        var fn = config.GetProperty("onChange");
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
        var param = new BoolParamDeclaration(name, start);
        context.GetModuleContext().RegisterDisposable(param);
        var fn = config.GetProperty("onChange");
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
        var param = new ActionDeclaration(name);
        param.OnTrigger(context, fn);
        context.GetModuleContext().RegisterDisposable(param);
        return param;
    }
}
