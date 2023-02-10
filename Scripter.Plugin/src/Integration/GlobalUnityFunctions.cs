using ScripterLang;

public static class GlobalUnityFunctions
{
    private static readonly TimeReference _timeReference = new TimeReference();
    private static readonly RandomReference _randomReference = new RandomReference();

    public static void Register(GlobalLexicalContext lexicalContext)
    {
        lexicalContext.DeclareGlobal("Time", _timeReference);
        lexicalContext.DeclareGlobal("Random", _randomReference);
    }
}
