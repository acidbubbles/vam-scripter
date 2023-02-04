using System;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class ScriptsManagerScreen : Screen
{
    public static Screen Create(ScreenManager manager)
    {
        var screen = Screen.Create<ScriptsManagerScreen>(manager.Root);

        var button = Instantiate(manager.Prefabs.configurableButtonPrefab, screen.transform).GetComponent<UIDynamicButton>();
        button.label = "Edit";
        button.button.onClick.AddListener(() =>
        {
            manager.EditScript(manager.Scripts.ByName("1"));
        });

        return screen;
    }
}
