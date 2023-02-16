using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class WelcomeView : MonoBehaviour
{
    private const string _welcomeText = @"Welcome to Scripter! Don't worry, if you have some basic understanding of JavaScript, you'll be able to use this plugin in no time.

Check out these templates to get started.";

    public static WelcomeView Create(Transform parent, ScripterUI ui)
    {
        var go = new GameObject();
        go.transform.SetParent(parent, false);
        go.SetActive(false);

        {
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = new Vector2(0, 0);
            rect.sizeDelta = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);

            var bg = go.AddComponent<Image>();
            bg.raycastTarget = false;
            bg.color = new Color(30 / 255f, 30 / 255f, 30 / 255f);

            var verticalLayout = go.AddComponent<VerticalLayoutGroup>();
            verticalLayout.spacing = 10f;
            verticalLayout.childControlHeight = true;
            verticalLayout.childForceExpandHeight = false;
            verticalLayout.padding = new RectOffset(40, 40, 40, 40);
        }

        var screen = go.AddComponent<WelcomeView>();

        CreateWelcomeText(go);

        {
            var templates = new GameObject();
            templates.transform.SetParent(go.transform, false);

            var layout = templates.AddComponent<LayoutElement>();
            layout.flexibleWidth = 1;
            layout.flexibleHeight = 1;

            var grid = templates.AddComponent<GridLayoutGroup>();
            grid.spacing = new Vector2(60, 60);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.childAlignment = TextAnchor.MiddleCenter;
            grid.cellSize = new Vector2(400, 200);

            var fitter = templates.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            AddTemplateButton(templates.transform, "Scene triggers", () =>
            {
                Scripter.Singleton.ProgramFiles.Clear();
                Scripter.Singleton.ProgramFiles.Create(
                    "index.js",
                    @"import { self } from ""scripter"";

let valueParam = self.declareFloatParam({
    name: ""Value"",
    default: 0,
    min: 0,
    max: 1,
    constrain: true
});

valueParam.onChange(value => {
    console.log(""Value changed to: "", value);
});
");
                Scripter.Singleton.ProgramFiles.Apply();
            });
            AddTemplateButton(templates.transform, "Respond to a\nKeybindings event", () => { });
            AddTemplateButton(templates.transform, "Run code every frame", () => { });
            AddTemplateButton(templates.transform, "Start from scratch", () =>
            {
                Scripter.Singleton.ProgramFiles.Create(
                    "index.js",
                    @"import { self } from ""scripter"";

// Start writing your code here!
");
            });
            AddTemplateButton(templates.transform, "Open the documentation\n(web browser)",
                () => Application.OpenURL("https://github.com/acidbubbles/vam-scripter/blob/master/README.md"));
            AddTemplateButton(templates.transform, "Support this plugin\n(web browser)",
                () => Application.OpenURL("https://www.patreon.com/acidbubbles"));
        }

        return screen;
    }

    private static void CreateWelcomeText(GameObject go)
    {
        var welcomeText = new GameObject();
        welcomeText.transform.SetParent(go.transform, false);

        var layout = welcomeText.AddComponent<LayoutElement>();
        layout.flexibleWidth = 1;
        layout.minHeight = layout.preferredHeight = 160;

        var text = welcomeText.AddComponent<Text>();
        text.font = SuperController.singleton.dynamicButtonPrefab.GetComponentInChildren<Text>(true).font;
        text.fontSize = 28;
        text.text = _welcomeText;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;
        text.color = Color.white;
        text.supportRichText = true;
    }

    private static void AddTemplateButton(Transform parent, string label, UnityAction act)
    {
        var go = new GameObject();
        go.transform.SetParent(parent, false);

        var layout = go.AddComponent<LayoutElement>();
        layout.preferredWidth = 600;
        layout.preferredHeight = 600;

        var bg = go.AddComponent<Image>();
        bg.color = new Color(50 / 255f, 50 / 255f, 50 / 255f);

        go.AddComponent<Clickable>().onClick.AddListener(_ => act());

        {
            var labelGo = new GameObject();
            labelGo.transform.SetParent(go.transform, false);

            var rect = labelGo.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = new Vector2(0, 0);
            rect.sizeDelta = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);

            // labelGo.AddComponent<Image>().color = Color.green;

            var text = labelGo.AddComponent<Text>();
            text.font = SuperController.singleton.dynamicButtonPrefab.GetComponentInChildren<Text>(true).font;
            text.fontSize = 28;
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;
            text.color = Color.white;
        }
    }
}
