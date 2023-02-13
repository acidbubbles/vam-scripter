using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class ScripterTabsList : MonoBehaviour
{
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

    public ScripterTab AddTab(string label)
    {
        return ScripterTab.Create(transform, label);
    }
}
