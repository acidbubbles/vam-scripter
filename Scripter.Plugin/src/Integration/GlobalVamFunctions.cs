using ScripterLang;

public static class GlobalVamFunctions
{
    public static void Register(GlobalLexicalContext lexicalContext)
    {
        #warning Play audio clips
        lexicalContext.Globals.Add("scene", new SceneReference());
        lexicalContext.Globals.Add("console", new ConsoleReference());
        #warning Add Keybindings invoke
    }
}
