using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
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
        screen.CreateMultilineInput(script);

        var toolbar = UIUtils.MakeToolbar(screen.transform, 270);
        script.history.undoButton = UIUtils.CreateToolbarButton(toolbar, "\u21BA", 40, true, script.history.Undo);
        script.history.redoButton = UIUtils.CreateToolbarButton(toolbar, "\u21BB", 40, true, script.history.Redo);
        script.history.UpdateButtons();
        UIUtils.CreateToolbarButton(toolbar, "Validate", 120, false, script.Parse);

        return screen;
    }

    private static CodeInputField _input;

    private void CreateMultilineInput(Script script)
    {
        var textfield = Instantiate(Scripter.singleton.manager.configurableTextFieldPrefab, transform).GetComponent<UIDynamicTextField>();
        textfield.backgroundColor = new Color(30 / 255f, 30 / 255f, 30 / 255f);
        textfield.textColor = new Color(156 / 255f, 220 / 255f, 254 / 255f);
        script.sourceJSON.dynamicText = textfield;

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
        script.input = input;
        input.textComponent = textfield.UItext;
        input.lineType = InputField.LineType.MultiLineNewline;
        script.sourceJSON.inputField = input;
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
