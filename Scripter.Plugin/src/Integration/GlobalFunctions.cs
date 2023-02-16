using ScripterLang;

public static class GlobalFunctions
{
    public static void Register(GlobalLexicalContext lexicalContext)
    {
        #warning Play audio clips
        lexicalContext.DeclareGlobal("console", new ConsoleReference());
        lexicalContext.DeclareModule(new ScripterModule());
        #warning Add Keybindings invoke
    }
}
