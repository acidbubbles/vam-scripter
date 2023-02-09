using ScripterLang;

public static class GlobalVamFunctions
{
    public static void Register(GlobalLexicalContext lexicalContext)
    {
        #warning Play audio clips
        lexicalContext.DeclareHoisted("scene", new SceneReference());
        lexicalContext.DeclareHoisted("console", new ConsoleReference());
        #warning Add Keybindings invoke
    }
}
