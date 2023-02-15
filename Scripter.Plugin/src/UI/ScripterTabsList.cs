using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class ScripterTabsList : MonoBehaviour
{
    private readonly List<ScripterTab> _tabs = new List<ScripterTab>();

    public static ScripterTabsList Create(Transform parent)
    {
        var go = new GameObject();
        go.transform.SetParent(parent, false);

        var layout = go.AddComponent<LayoutElement>();
        layout.preferredHeight = 30;
        layout.flexibleWidth = 1;

        var group = go.AddComponent<HorizontalLayoutGroup>();
        group.spacing = 10f;
        group.childControlWidth = true;
        group.childForceExpandWidth = false;
        group.childControlHeight = true;
        group.childForceExpandHeight = false;
        group.childAlignment = TextAnchor.UpperLeft;

        var tabs = go.AddComponent<ScripterTabsList>();

        var bg = go.AddComponent<Image>();
        bg.color = new Color(59 / 255f, 59 / 255f, 59 / 255f);

        return tabs;
    }

    public ScripterTab AddTab(string label, Transform content)
    {
        var tab = ScripterTab.Create(transform, label, content);
        tab.transform.SetSiblingIndex(tab.transform.parent.childCount - 2);
        _tabs.Add(tab);
        tab.clickable.onClick.AddListener(_ => SelectTab(tab));
        return tab;
    }

    public void SetLastTab(string label, Transform content)
    {
        var tab = ScripterTab.Create(transform, label, content);
        _tabs.Add(tab);
        tab.clickable.onClick.AddListener(_ => SelectTab(tab));
    }

    public void SelectTab(ScripterTab tab)
    {
        foreach (var t in _tabs)
        {
            t.Selected = t == tab;
        }
    }

    public void RemoveTab(ScripterTab tab)
    {
        #warning Test (add a way to remove something)
        Destroy(tab.content.gameObject);
        Destroy(tab.gameObject);
    }
}
