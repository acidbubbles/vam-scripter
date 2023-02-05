using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Screen : MonoBehaviour
{
    protected static T Create<T>(Transform parent) where T : Screen
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

        return go.AddComponent<T>();
    }

    protected static Transform MakeToolbar(Transform parent, float height = 50)
    {
        var go = new GameObject();
        go.transform.SetParent(parent, false);
        var layout = go.AddComponent<LayoutElement>();
        layout.minHeight = height;
        layout.preferredHeight = height;
        layout.flexibleHeight = 0;
        var group = go.gameObject.AddComponent<HorizontalLayoutGroup>();
        group.spacing = 4f;
        group.childForceExpandWidth = false;
        group.childControlWidth = true;
        return go.transform;
    }

    protected static void AddToToolbar(Transform parent, Transform item)
    {
        var layoutElement = item.GetComponent<LayoutElement>();
        layoutElement.preferredWidth = 0;
        layoutElement.flexibleWidth = 100;
    }

    protected static Transform CreateButton(Transform parent, Transform prefab, string label, UnityAction action)
    {
        var button = Instantiate(prefab, parent, false);
        var ui = button.GetComponent<UIDynamicButton>();
        ui.label = label;
        ui.button.onClick.AddListener(action);
        return button;
    }
}
