using UnityEngine;
using UnityEngine.UI;

public class ScripterUI : MonoBehaviour
{
    public static ScripterUI Create(Transform parent, Scripter scripter)
    {
        var go = new GameObject();
        go.transform.SetParent(parent, false);

        var ui = go.AddComponent<ScripterUI>();

        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.anchoredPosition = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(0, 0);
        rect.offsetMin = new Vector2(0, 60);
        rect.offsetMax = new Vector2(0, 0);

        var group = go.AddComponent<VerticalLayoutGroup>();
        group.spacing = 0f;
        group.childControlHeight = true;
        group.childForceExpandHeight = false;
        group.childAlignment = TextAnchor.UpperLeft;

        var bg = go.AddComponent<Image>();
        bg.raycastTarget = false;
        bg.color = Color.black;

        ui._tabs = ScripterTabsList.Create(go.transform);

        var content = new GameObject();
        content.transform.SetParent(go.transform, false);

        var layout = content.AddComponent<LayoutElement>();
        layout.preferredHeight = 1000f;
        layout.flexibleWidth = 1;

        ui._content = content;

        var createTab = CreateView.Create(content.transform, ui);
        ui._tabs.SetLastTab("+", createTab.transform);

        ui.CreateConsole(go.transform);

        return ui;
    }

    private ScripterTabsList _tabs;

    private void CreateConsole(Transform parent)
    {
        _console = Instantiate(Scripter.Singleton.manager.configurableTextFieldPrefab, parent, false).GetComponent<UIDynamicTextField>();
        _console.backgroundColor = Color.black;
        _console.textColor = Color.white;
        Scripter.Singleton.Console.Init(_console);

        _scrollRect = _console.transform.Find("Scroll View").GetComponent<ScrollRect>();

        Scripter.Singleton.Console.ConsoleJSON.setCallbackFunction = OnConsoleChange;

        // ReSharper disable once Unity.InefficientPropertyAccess
        var toolbar = UIUtils.MakeToolbar(_console.transform, 100);
        UIUtils.CreateToolbarButton(toolbar, "Clear", 40, false, () => { Scripter.Singleton.Console.Clear(); });
    }

    private void OnConsoleChange(string val)
    {
        Invoke(nameof(ScrollToBottom), 0);
    }

    private void ScrollToBottom()
    {
        _scrollRect.verticalNormalizedPosition = 0;
    }

    private GameObject _content;
    private UIDynamicTextField _console;
    private ScrollRect _scrollRect;

    public ScripterTab AddWelcomeTab()
    {
        var welcome = WelcomeView.Create(_content.transform, this);
        return _tabs.AddTab("Welcome", welcome.transform);
    }

    public ScripterTab AddScriptTab(Script script)
    {
        var editor = CodeEditorView.Create(_content.transform, script);
        return _tabs.AddTab(script.NameJSON.val, editor.transform);
    }

    public void SelectTab(ScripterTab tab)
    {
        _tabs.SelectTab(tab);
    }

    public void RemoveTab(ScripterTab tab)
    {
        _tabs.RemoveTab(tab);
    }
}
