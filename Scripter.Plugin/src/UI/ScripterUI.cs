using UnityEngine;
using UnityEngine.UI;

public class ScripterUI : MonoBehaviour
{
    public static ScripterUI Create(Transform parent, Scripter scripter)
    {
        var go = new GameObject();
        go.transform.SetParent(parent, false);

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

        var tabs = ScriptsTabs.Create(go.transform);

        var editor = ScriptEditor.Create(go.transform, null);

        return go.AddComponent<ScripterUI>();
    }
}
