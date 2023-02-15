using UnityEngine;
using UnityEngine.UI;

public class ScripterTab : MonoBehaviour
{
    public static ScripterTab Create(Transform parent, string label, Transform content)
    {
        const int padding = 20;

        var go = new GameObject();
        go.transform.SetParent(parent, false);

        var tab = go.AddComponent<ScripterTab>();
        tab.content = content;

        {
            var layout = go.AddComponent<LayoutElement>();
            layout.minHeight = layout.preferredHeight = 70;

            var group = go.AddComponent<HorizontalLayoutGroup>();
            group.childControlHeight = true;
            group.childForceExpandHeight = false;
            group.childControlWidth = true;
            group.childForceExpandWidth = false;
            group.padding = new RectOffset(padding, padding, 0, 0);

            var fitter = go.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            #warning Roll over
            var tabBg = go.AddComponent<Image>();
            tabBg.raycastTarget = true;
            tab._bg = tabBg;

            var clickable = go.AddComponent<Clickable>();
            tab.clickable = clickable;
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

            var filenameLayout = filename.AddComponent<LayoutElement>();
            filenameLayout.minHeight = filenameLayout.preferredHeight = 70;

            var text = filename.AddComponent<Text>();
            #warning Link to NameJSON, double-click to edit
            text.font = SuperController.singleton.dynamicButtonPrefab.GetComponentInChildren<Text>(true).font;
            text.fontSize = 28;
            text.text = label;
            text.alignment = TextAnchor.MiddleLeft;
            text.raycastTarget = false;
            tab._text = text;

            {
                var selected = new GameObject();
                selected.transform.SetParent(filename.transform, false);

                var selectedRect = selected.AddComponent<RectTransform>();
                selectedRect.anchorMin = Vector2.zero;
                selectedRect.anchorMax = new Vector2(1, 0);
                selectedRect.anchoredPosition = new Vector2(0.5f, 0);
                selectedRect.pivot = new Vector2(0.5f, 0);
                selectedRect.sizeDelta = new Vector2(padding * 2, 10);

                var selectedUnderline = selected.AddComponent<Image>();
                selectedUnderline.raycastTarget = false;
                tab._underline = selectedUnderline;
            }
        }

        tab.SetUnselected();

        return tab;
    }

    public Transform content;
    public Clickable clickable;

    private Image _bg;
    private Text _text;
    private Image _underline;

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
        content.gameObject.SetActive(true);
        _text.color = new Color(192 / 255f, 192 / 255f, 198 / 255f);
        _bg.color = new Color(50f / 255f, 50f / 255f, 50f / 255f);
        _underline.color = new Color(76 / 255f, 103 / 255f, 158 / 255f);
    }

    public void SetUnselected()
    {
        content.gameObject.SetActive(false);
        _text.color = new Color(192 / 255f, 192 / 255f, 198 / 255f);
        _bg.color = new Color(59 / 255f, 59 / 255f, 59 / 255f);
        _underline.color = new Color(59 / 255f, 59 / 255f, 59 / 255f);
    }
}
