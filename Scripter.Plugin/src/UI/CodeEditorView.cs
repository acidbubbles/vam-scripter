using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class CodeEditorView : MonoBehaviour
{
    public static CodeEditorView Create(Transform parent, Script script)
    {
        var go = new GameObject();
        go.transform.SetParent(parent, false);
        go.SetActive(false);

        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.anchoredPosition = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(0, 0);
        rect.offsetMin = new Vector2(0, 0);
        rect.offsetMax = new Vector2(0, 0);

        var bg = go.AddComponent<Image>();
        bg.raycastTarget = false;
        bg.color = new Color(30 / 255f, 30 / 255f, 30 / 255f);

        var screen = go.AddComponent<CodeEditorView>();
        screen.CreateMultilineInput(script.SourceJSON);

        var toolbar = MakeToolbar(screen.transform);
        CreateToolbarButton(toolbar, "\u21BA", script.History.Undo);
        CreateToolbarButton(toolbar, "\u21BB", script.History.Redo);

        return screen;
    }

    private static Transform MakeToolbar(Transform parent, float height = 50)
    {
        var go = new GameObject();
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.sizeDelta = new Vector2(180, 72);
        rect.offsetMax = new Vector2(-20, -20);

        var group = go.gameObject.AddComponent<HorizontalLayoutGroup>();
        group.spacing = 4f;
        group.childForceExpandWidth = false;
        group.childControlWidth = true;

        return go.transform;
    }

    private static void CreateToolbarButton(Transform parent, string label, UnityAction action)
    {
        var button = Instantiate(Scripter.Singleton.manager.configurableButtonPrefab, parent, false);

        var ui = button.GetComponent<UIDynamicButton>();
        ui.label = label;
        ui.button.onClick.AddListener(action);
        ui.buttonText.fontStyle = FontStyle.Bold;
        ui.buttonText.fontSize = 44;

        var layoutElement = button.GetComponent<LayoutElement>();
        layoutElement.minWidth = layoutElement.preferredWidth = 40;
    }

    private static CodeInputField _input;

    private void CreateMultilineInput(JSONStorableString jss)
    {
        var textfield = Instantiate(Scripter.Singleton.manager.configurableTextFieldPrefab, transform).GetComponent<UIDynamicTextField>();
        textfield.backgroundColor = new Color(30 / 255f, 30 / 255f, 30 / 255f);
        textfield.textColor = new Color(156 / 255f, 220 / 255f, 254 / 255f);
        jss.dynamicText = textfield;

        Destroy(textfield.GetComponent<LayoutElement>());

        var rect = textfield.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(0, 0);
        rect.pivot = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(0, 0);
        rect.offsetMin = new Vector2(0, 0);

        var text = textfield.GetComponentInChildren<Text>(true);

        var input = text.gameObject.AddComponent<CodeInputField>();
        input.textComponent = textfield.UItext;
        input.lineType = InputField.LineType.MultiLineNewline;
        jss.inputField = input;
        _input = input;

        textfield.gameObject.AddComponent<Clickable>().onClick.AddListener(OnClick);
    }

    private static void OnClick(PointerEventData _)
    {
        EventSystem.current.SetSelectedGameObject(_input.gameObject, null);
        _input.ActivateInputField();
        _input.StartCoroutine(Select());
    }

    private static IEnumerator Select()
    {
        yield return 0;
        _input.MoveTextEnd(true);
    }
}
