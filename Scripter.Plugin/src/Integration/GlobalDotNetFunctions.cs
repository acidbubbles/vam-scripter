using ScripterLang;

public static class GlobalDotNetFunctions
{
    public static void Register(GlobalLexicalContext lexicalContext)
    {
        lexicalContext.Globals.Add("DateTime", new DateTimeClassReference());
    }
}
