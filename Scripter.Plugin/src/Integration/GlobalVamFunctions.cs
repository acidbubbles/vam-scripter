using System;
using ScripterLang;

public static class GlobalVamFunctions
{
    public static void Register(GlobalLexicalContext lexicalContext)
    {
        #warning Play audio clips
        lexicalContext.Globals.Add("scene", new SceneReference());
        lexicalContext.Globals.Add("console", new ConsoleReference());
        lexicalContext.Functions.Add("invokeKeybinding", InvokeKeybinding);
    }

    private static Value InvokeKeybinding(RuntimeDomain domain, Value[] args)
    {
        throw new NotImplementedException("Invoking keybindings shortcuts is not yet implemented");
    }
}
