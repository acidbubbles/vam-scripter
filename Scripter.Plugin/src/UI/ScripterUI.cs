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

        var layout = go.AddComponent<LayoutElement>();
        layout.preferredHeight = 1200f;
        layout.flexibleWidth = 1;

        var group = go.AddComponent<VerticalLayoutGroup>();
        group.spacing = 10f;
        group.childControlHeight = true;
        group.childForceExpandHeight = false;

        var fitter = go.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        return go.AddComponent<ScripterUI>();
    }
}
