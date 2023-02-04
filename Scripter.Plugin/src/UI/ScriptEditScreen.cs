using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Events;
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
        button.button.onClick.AddListener(script.Run);
        CreateMultilineInput(screen.transform, manager.Prefabs, script.SourceJSON);

        var toolbar = MakeToolbar(screen.transform);
        AddToToolbar(toolbar, manager.Prefabs.configurableButtonPrefab, "Undo", script.History.Undo);
        AddToToolbar(toolbar, manager.Prefabs.configurableButtonPrefab, "Redo", script.History.Redo);

        var console = Instantiate(manager.Prefabs.configurableTextFieldPrefab, screen.transform).GetComponent<UIDynamicTextField>();
        console.backgroundColor = Color.black;
        console.textColor = Color.white;
        script.ConsoleJSON.dynamicText = console;

        return screen;
    }

    private static void CreateMultilineInput(Transform parent, MVRPluginManager prefabs, JSONStorableString jss)
    {
        var textfield = Instantiate(prefabs.configurableTextFieldPrefab, parent).GetComponent<UIDynamicTextField>();
        textfield.height = 880;
        jss.dynamicText = textfield;
        textfield.backgroundColor = Color.white;
        var text = textfield.GetComponentInChildren<Text>(true);
        var input = text.gameObject.AddComponent<InputField>();
        input.lineType = InputField.LineType.MultiLineNewline;
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

    private static Transform MakeToolbar(Transform parent)
    {
        var go = new GameObject();
        go.transform.SetParent(parent, false);
        var layout = go.AddComponent<LayoutElement>();
        layout.minHeight = 50;
        layout.preferredHeight = 50;
        layout.flexibleHeight = 0;
        var group = go.gameObject.AddComponent<HorizontalLayoutGroup>();
        group.spacing = 4f;
        group.childForceExpandWidth = false;
        group.childControlWidth = true;
        return go.transform;
    }

    private static void AddToToolbar(Transform parent, Transform prefab, string label, UnityAction action)
    {
        var button = Object.Instantiate(prefab, parent, false);
        var ui = button.GetComponent<UIDynamicButton>();
        ui.label = label;
        ui.button.onClick.AddListener(action);
        var layoutElement = button.GetComponent<LayoutElement>();
        layoutElement.preferredWidth = 0;
        layoutElement.flexibleWidth = 100;
    }
}
