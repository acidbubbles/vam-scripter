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
        //
        // var layout = go.AddComponent<LayoutElement>();
        // layout.preferredHeight = 1200f;
        // layout.flexibleWidth = 1;

        var group = go.AddComponent<VerticalLayoutGroup>();
        group.spacing = 0f;
        group.childControlHeight = true;
        group.childForceExpandHeight = false;
        group.childAlignment = TextAnchor.UpperLeft;

        var bg = go.AddComponent<Image>();
        bg.raycastTarget = false;
        bg.color = Color.black;

        // var fitter = go.AddComponent<ContentSizeFitter>();
        // fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ui._tabs = ScripterTabsList.Create(go.transform);

        var content = new GameObject();
        content.transform.SetParent(go.transform, false);

        var layout = content.AddComponent<LayoutElement>();
        layout.preferredHeight = 1200f;
        layout.flexibleWidth = 1;

        ui._content = content;

        return ui;
    }

    private ScripterTabsList _tabs;
    private GameObject _content;

    public ScripterTab AddWelcomeTab()
    {
        var welcome = WelcomeView.Create(_content.transform, this);
        return _tabs.AddTab("Welcome", welcome.transform);
    }

    public ScripterTab AddScriptTab(Script script)
    {
        var editor = CodeEditorView.Create(_content.transform, script);
        return _tabs.AddTab("index.js", editor.transform);
    }

    public void SelectTab(ScripterTab tab)
    {
        _tabs.SelectTab(tab);
    }
}
