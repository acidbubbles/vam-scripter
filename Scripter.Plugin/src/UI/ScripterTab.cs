using UnityEngine;
using UnityEngine.UI;

public class ScripterTab : MonoBehaviour
{
    public static ScripterTab Create(Transform parent, string label)
    {
        var go = new GameObject();
        go.transform.SetParent(parent, false);

        var tab = go.AddComponent<ScripterTab>();

        {
            var layout = go.AddComponent<LayoutElement>();
            layout.minHeight = layout.preferredHeight = 70;
            layout.minWidth = layout.preferredWidth = 200;

            var tabBg = go.AddComponent<Image>();
            tabBg.raycastTarget = true;
            tab.bg = tabBg;
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
            rect.offsetMin = new Vector2(20, 5);

            var text = filename.AddComponent<Text>();
            text.font = SuperController.singleton.dynamicButtonPrefab.GetComponentInChildren<Text>(true).font;
            text.fontSize = 28;
            text.text = label;
            text.alignment = TextAnchor.MiddleLeft;
            text.raycastTarget = false;
            tab.text = text;
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
            tab.underline = bg;
        }

        tab.SetUnselected();

        return tab;
    }

    public Image bg;

    public Text text;

    public Image underline;

    private bool _selected;

    public bool Selected
    {
        get { return _selected; }
        set
        {
            if (_selected == value) return;
            _selected = value;
            if(value)
                SetSelected();
            else
                SetUnselected();

        }
    }

    public void SetSelected()
    {
        text.color = new Color(192 / 255f, 192 / 255f, 198 / 255f);
        bg.color = new Color(50f / 255f, 50f / 255f, 50f / 255f);
        underline.color = new Color(76 / 255f, 103 / 255f, 158 / 255f);
    }

    public void SetUnselected()
    {
        text.color = new Color(192 / 255f, 192 / 255f, 198 / 255f);
        bg.color = new Color(59 / 255f, 59 / 255f, 59 / 255f);
        underline.color = new Color(59 / 255f, 59 / 255f, 59 / 255f);
    }
}
