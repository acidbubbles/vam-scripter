using ScripterLang;

public static class GlobalFunctions
{
    public static void Register(GlobalLexicalContext lexicalContext)
    {
        GlobalDotNetFunctions.Register(lexicalContext);
        GlobalUnityFunctions.Register(lexicalContext);
        GlobalVamFunctions.Register(lexicalContext);
    }
}
