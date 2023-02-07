using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ScripterLang;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class ScriptEditScreen : Screen
{
    public static Screen Create(ScreenManager manager, Script script)
    {
        var screen = Screen.Create<ScriptEditScreen>(manager.Root);

        var button = Instantiate(manager.Prefabs.configurableButtonPrefab, screen.transform).GetComponent<UIDynamicButton>();
        button.label = "Run";
        button.button.onClick.AddListener(() => RunScriptWithTimer(script));
        CreateMultilineInput(screen.transform, manager.Prefabs, script.SourceJSON);

        var toolbar = MakeToolbar(screen.transform);
        AddToToolbar(toolbar, CreateButton(toolbar, manager.Prefabs.configurableButtonPrefab, "Undo", script.History.Undo));
        AddToToolbar(toolbar, CreateButton(toolbar, manager.Prefabs.configurableButtonPrefab, "Redo", script.History.Redo));

        var console = Instantiate(manager.Prefabs.configurableTextFieldPrefab, screen.transform).GetComponent<UIDynamicTextField>();
        console.backgroundColor = Color.black;
        console.textColor = Color.white;
        script.ConsoleJSON.dynamicText = console;

        return screen;
    }

    private static void RunScriptWithTimer(Script script)
    {
        script.ConsoleJSON.val = $"[{DateTime.Now.ToLongTimeString()}] Running...";
        var sw = new Stopwatch();
        sw.Start();
        script.Run(Value.Undefined);
        sw.Stop();
        script.ConsoleJSON.val += $"\n[{DateTime.Now.ToLongTimeString()}] <color=green>Script completed in {sw.Elapsed.TotalMilliseconds:0.000}ms</color>";
    }

    private static void CreateMultilineInput(Transform parent, MVRPluginManager prefabs, JSONStorableString jss)
    {
        var textfield = Instantiate(prefabs.configurableTextFieldPrefab, parent).GetComponent<UIDynamicTextField>();
        textfield.height = 880;
        jss.dynamicText = textfield;
        textfield.backgroundColor = Color.white;
        var text = textfield.GetComponentInChildren<Text>(true);
        var input = text.gameObject.AddComponent<CodeInputField>();
        input.textComponent = textfield.UItext;
        jss.inputField = input;
        textfield.gameObject.AddComponent<Clickable>().onClick.AddListener(_ =>
        {
            if (jss.val == jss.defaultVal) jss.val = "";
            EventSystem.current.SetSelectedGameObject(input.gameObject, null);
            input.ActivateInputField();
            input.StartCoroutine(Select(input));
        });
    }

    private static IEnumerator Select(InputField input)
    {
        yield return 0;
        input.MoveTextEnd(true);
    }
}
