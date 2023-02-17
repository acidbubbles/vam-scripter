using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class UIUtils
{
    public static Transform MakeToolbar(Transform parent, float width)
    {
        var go = new GameObject();
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.sizeDelta = new Vector2(width, 72);
        rect.offsetMax = new Vector2(-20, -20);

        var group = go.gameObject.AddComponent<HorizontalLayoutGroup>();
        group.spacing = 4f;
        group.childForceExpandWidth = false;
        group.childControlWidth = true;

        return go.transform;
    }

    public static UIDynamicButton CreateToolbarButton(Transform parent, string label, float width, bool icon, UnityAction action)
    {
        var button = Object.Instantiate(Scripter.singleton.manager.configurableButtonPrefab, parent, false);

        var ui = button.GetComponent<UIDynamicButton>();
        ui.label = label;
        ui.button.onClick.AddListener(action);
        ui.buttonText.fontStyle = icon ? FontStyle.Bold : FontStyle.Normal;
        ui.buttonText.fontSize = icon ? 44 : 24;

        var layoutElement = button.GetComponent<LayoutElement>();
        layoutElement.minWidth = layoutElement.preferredWidth = width;

        return ui;
    }
}
