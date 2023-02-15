using ScripterLang;

public static class GlobalFunctions
{
    private static readonly TimeReference _timeReference = new TimeReference();
    private static readonly RandomReference _randomReference = new RandomReference();
    private static readonly DateTimeClassReference _dateTimeClassReference = new DateTimeClassReference();

    public static void Register(GlobalLexicalContext lexicalContext)
    {
        #warning use import
        #warning Play audio clips
        lexicalContext.DeclareGlobal("scene", new SceneReference());
        lexicalContext.DeclareGlobal("console", new ConsoleReference());
        lexicalContext.DeclareGlobal("Time", _timeReference);
        lexicalContext.DeclareGlobal("Random", _randomReference);
        lexicalContext.DeclareGlobal("DateTime", _dateTimeClassReference);
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
        ValidateArgumentsLength(nameof(DeclareFloatParam), args, 1);
        var config = args[0].AsObject;
        var name = config.GetProperty("name").AsString;
        var start = config.GetProperty("default").AsNumber;
        var min = config.GetProperty("min").AsNumber;
        var max = config.GetProperty("max").AsNumber;
        var constrain = config.GetProperty("constrain").AsBool;
        return new ScripterFloatParam(name, start, min, max, constrain);
    }
}
