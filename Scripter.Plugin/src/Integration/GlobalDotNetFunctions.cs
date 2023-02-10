using ScripterLang;

public static class GlobalDotNetFunctions
{
    private static readonly DateTimeClassReference _dateTimeClassReference = new DateTimeClassReference();

    public static void Register(GlobalLexicalContext lexicalContext)
    {
        #warning Instead use "Date" like in JavaScript
        lexicalContext.DeclareGlobal("DateTime", _dateTimeClassReference);
    }
}
