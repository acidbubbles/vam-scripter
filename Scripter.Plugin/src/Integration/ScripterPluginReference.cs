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

    private Value DeclareFloatParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(DeclareFloatParam), args, 1);
#warning Default values (HasProperty)
        var config = args[0].AsObject;
        var name = config.GetProperty("name").AsString;
        var start = config.GetProperty("default").AsNumber;
        var min = config.GetProperty("min").AsNumber;
        var max = config.GetProperty("max").AsNumber;
        var constrain = config.GetProperty("constrain").AsBool;
        var param = new ScripterFloatParamDeclaration(name, start, min, max, constrain);
        context.GetModuleContext().RegisterDisposable(param);
        return param;
    }

    private Value DeclareStringParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(DeclareStringParam), args, 1);
        var config = args[0].AsObject;
        var name = config.GetProperty("name").AsString;
        var start = config.GetProperty("default").AsString;
        var param = new ScripterStringParamDeclaration(name, start);
        context.GetModuleContext().RegisterDisposable(param);
        return param;
    }

    private Value DeclareBoolParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(DeclareBoolParam), args, 1);
        var config = args[0].AsObject;
        var name = config.GetProperty("name").AsString;
        var start = config.GetProperty("default").AsBool;
        var param = new ScripterBoolParamDeclaration(name, start);
        context.GetModuleContext().RegisterDisposable(param);
        return param;
    }

    private Value DeclareAction(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(DeclareAction), args, 1);
#warning Allow just passing the name instead
        var config = args[0].AsObject;
        var name = config.GetProperty("name").AsString;
        var param = new ScripterActionDeclaration(name);
        context.GetModuleContext().RegisterDisposable(param);
        return param;
    }
}
