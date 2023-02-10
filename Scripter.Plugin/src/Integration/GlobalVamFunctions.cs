using ScripterLang;

public static class GlobalVamFunctions
{
    public static void Register(GlobalLexicalContext lexicalContext)
    {
        #warning use import
        #warning Play audio clips
        lexicalContext.DeclareGlobal("scene", new SceneReference());
        lexicalContext.DeclareGlobal("console", new ConsoleReference());
        #warning Add Keybindings invoke
    }
}
