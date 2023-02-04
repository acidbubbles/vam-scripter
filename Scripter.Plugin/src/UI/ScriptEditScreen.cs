using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScriptEditScreen : Screen
{
    public static Screen Create(Transform parent, MVRPluginManager manager, Script script)
    {
        var go = new GameObject();
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.anchoredPosition = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(0, 0);

        var layout = go.AddComponent<LayoutElement>();
        layout.preferredHeight = 1200f;
        layout.flexibleWidth = 1;

        var screen = go.AddComponent<ScriptEditScreen>();

        var group = go.AddComponent<VerticalLayoutGroup>();
        group.spacing = 10f;
        group.childControlHeight = true;
        group.childForceExpandHeight = false;

        var fitter = go.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var button = Instantiate(manager.configurableButtonPrefab, go.transform).GetComponent<UIDynamicButton>();
        button.label = "Run";
        button.button.onClick.AddListener(script.Run);
        CreateMultilineInput(go.transform, manager, script.SourceJSON);

        var toolbar = MakeToolbar(go.transform, manager);
        AddToToolbar(toolbar, manager.configurableButtonPrefab, "Undo", script.History.Undo);
        AddToToolbar(toolbar, manager.configurableButtonPrefab, "Redo", script.History.Redo);

        var console = Instantiate(manager.configurableTextFieldPrefab, go.transform).GetComponent<UIDynamicTextField>();
        console.backgroundColor = Color.black;
        console.textColor = Color.white;
        script.ConsoleJSON.dynamicText = console;

        return screen;
    }

    private static UIDynamicTextField CreateMultilineInput(Transform parent, MVRPluginManager manager, JSONStorableString jss)
    {
        var textfield = Instantiate(manager.configurableTextFieldPrefab, parent).GetComponent<UIDynamicTextField>();
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
        return textfield;
    }

    private static IEnumerator Select(InputField input)
    {
        yield return 0;
        input.MoveTextEnd(true);
    }

    private static Transform MakeToolbar(Transform parent, MVRPluginManager manager)
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
