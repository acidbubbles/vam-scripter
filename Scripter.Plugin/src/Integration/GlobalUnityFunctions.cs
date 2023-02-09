using ScripterLang;

public static class GlobalUnityFunctions
{
    private static readonly TimeReference _timeReference = new TimeReference();
    private static readonly RandomReference _randomReference = new RandomReference();

    public static void Register(GlobalLexicalContext lexicalContext)
    {
        lexicalContext.DeclareHoisted("Time", _timeReference);
        lexicalContext.DeclareHoisted("Random", _randomReference);
    }
}
