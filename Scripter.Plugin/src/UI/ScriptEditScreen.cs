using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ScripterLang;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class ScriptEditScreen : MonoBehaviour
{
    public static ScriptEditScreen Create(Transform parent, Script script)
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

        var group = go.AddComponent<VerticalLayoutGroup>();
        group.spacing = 10f;
        group.childControlHeight = true;
        group.childForceExpandHeight = false;

        var fitter = go.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var screen = go.AddComponent<ScriptEditScreen>();
        //
        // var button = Instantiate(manager.Prefabs.configurableButtonPrefab, screen.transform).GetComponent<UIDynamicButton>();
        // button.label = "Run";
        // button.button.onClick.AddListener(() => SuperController.LogMessage("Run!"));
        // CreateMultilineInput(screen.transform, manager.Prefabs, script.SourceJSON);
        //
        // var toolbar = MakeToolbar(screen.transform);
        // AddToToolbar(toolbar, CreateButton(toolbar, manager.Prefabs.configurableButtonPrefab, "Undo", script.History.Undo));
        // AddToToolbar(toolbar, CreateButton(toolbar, manager.Prefabs.configurableButtonPrefab, "Redo", script.History.Redo));
        //
        // var console = Instantiate(manager.Prefabs.configurableTextFieldPrefab, screen.transform).GetComponent<UIDynamicTextField>();
        // console.backgroundColor = Color.black;
        // console.textColor = Color.white;
        // manager.ConsoleJSON.dynamicText = console;

        return screen;
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
