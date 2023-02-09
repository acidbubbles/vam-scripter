using ScripterLang;

public static class GlobalDotNetFunctions
{
    private static readonly DateTimeClassReference _dateTimeClassReference = new DateTimeClassReference();

    public static void Register(GlobalLexicalContext lexicalContext)
    {
        lexicalContext.DeclareHoisted("DateTime", _dateTimeClassReference);
    }
}
