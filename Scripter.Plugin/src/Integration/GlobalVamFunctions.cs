using ScripterLang;

public static class GlobalVamFunctions
{
    public static void Register(GlobalLexicalContext lexicalContext)
    {
        #warning use import
        #warning Play audio clips
        lexicalContext.DeclareGlobal("scene", new SceneReference());
        lexicalContext.DeclareGlobal("console", new ConsoleReference());
        lexicalContext.DeclareModule(new ScripterModule());
        #warning Add Keybindings invoke
    }
}

public class ScripterPluginReference : ObjectReference
{
    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "declareFloatParam":
                return Func(DeclareFloatParam);
            default:
                return base.GetProperty(name);
        }
    }

    private Value DeclareFloatParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(DeclareFloatParam), args, 5);
        var name = args[0].Stringify;
        var start = args[1].AsNumber;
        var min = args[2].AsNumber;
        var max = args[3].AsNumber;
        var constrain = args[4].AsBool;
        return new ScripterFloatParam(name, start, min, max, constrain);
    }
}
