using ScripterLang;

public static class GlobalUnityFunctions
{
    private static readonly TimeReference _timeReference = new TimeReference();
    private static readonly RandomReference _randomReference = new RandomReference();

    public static void Register(GlobalLexicalContext lexicalContext)
    {
        lexicalContext.Globals.Add("Time", _timeReference);
        lexicalContext.Globals.Add("Random", _randomReference);
    }
}
