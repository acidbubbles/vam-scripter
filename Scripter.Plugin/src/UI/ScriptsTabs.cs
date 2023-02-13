using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class ScriptsTabs : MonoBehaviour
{
    public static ScriptsTabs Create(Transform parent)
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

        //
        AddTab(go.transform);
        //
        // var fitter = go.AddComponent<ContentSizeFitter>();
        // fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        var tabs = go.AddComponent<ScriptsTabs>();

        var bg = go.AddComponent<Image>();
        bg.color = new Color(59 / 255f, 59 / 255f, 59 / 255f);

        return tabs;
    }

    private static void AddTab(Transform parent)
    {
        var go = new GameObject();
        go.transform.SetParent(parent, false);

        {
            var layout = go.AddComponent<LayoutElement>();
            layout.minHeight = layout.preferredHeight = 70;
            layout.minWidth = layout.preferredWidth = 340;

            var tabBg = go.AddComponent<Image>();
            tabBg.raycastTarget = true;
            tabBg.color = new Color(50f / 255f, 50f / 255f, 50f / 255f);
        }

        {
            var filename = new GameObject();
            filename.transform.SetParent(go.transform, false);

            var rect = filename.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(0, 0);
            rect.pivot = new Vector2(0, 0);
            rect.sizeDelta = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 10);

            var text = filename.AddComponent<Text>();
            text.font = SuperController.singleton.dynamicButtonPrefab.GetComponentInChildren<Text>(true).font;
            text.fontSize = 28;
            text.text = " index.js";
            text.alignment = TextAnchor.MiddleLeft;
            text.color = new Color(192 / 255f, 192 / 255f, 198 / 255f);
            text.raycastTarget = false;
        }

        {
            var selected = new GameObject();
            selected.transform.SetParent(go.transform, false);

            var rect = selected.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = new Vector2(1, 0);
            rect.anchoredPosition = new Vector2(0.5f, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.sizeDelta = new Vector2(0, 10);

            var bg = selected.AddComponent<Image>();
            bg.raycastTarget = false;
            bg.color = new Color(96f / 255f, 100f / 255f, 105f / 255f);
        }
    }
}
