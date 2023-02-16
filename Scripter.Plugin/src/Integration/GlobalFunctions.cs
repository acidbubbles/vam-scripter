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
