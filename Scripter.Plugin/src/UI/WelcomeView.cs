using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class WelcomeView : MonoBehaviour
{
    private const string _welcomeText = @"Welcome to Scripter! Ff you have some basic understanding of JavaScript, you'll be able to use this plugin in no time.

Press the + button to create a file, or check out these templates to get started.";

    public static WelcomeView Create(Transform parent)
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

            // Scene triggers
            AddTemplateButton(templates.transform, "Scene triggers", () =>
            {
                Scripter.singleton.programFiles.DeleteAll();
                Scripter.singleton.programFiles.Create(
                    "index.js",
                    @"import { scripter, scene } from ""vam-scripter"";

// Get triggers from another atom
const timeline = scene
    .getAtom(""Person"")
    .getStorable(""plugin#0_VamTimeline.AtomPlugin"");

// Call a trigger
timeline.invokeAction(""Play Anim 1"");

// Get a float param
var speed = timeline.getFloatParam(""Speed"");

// Get and set the value
speed.val = speed.val + 0.1;

// Create a float trigger and react to events
let valueParam = scripter.declareFloatParam({
    name: ""Value"",
    min: 0,
    max: 1,
    onChange: value => {
        console.log(""Value changed to: "", value);
    }
});
");
                Scripter.singleton.programFiles.Run();
            });

            // Keybindings
            AddTemplateButton(templates.transform, "Respond to a\nKeybindings event", () =>
            {
                Scripter.singleton.programFiles.DeleteAll();
                Scripter.singleton.programFiles.Create(
                    "index.js",
                    @"import { keybindings } from ""vam-scripter"";

// Creates a Keybindings named ""Scripter.HelloWorld""
keybindings.declareCommand(""HelloWorld"", () => {
    console.log(""Hello from Keybindings!"");

    // You can also call other commands
    keybindings.invokeCommand(""Scripter.OpenUI"");
});");
            });

            // TODO: Use RotateTowards and MoveTo
            AddTemplateButton(templates.transform, "Run code every frame", () =>
            {
                Scripter.singleton.programFiles.DeleteAll();
                Scripter.singleton.programFiles.Create(
                    "index.js",
                    @"import { scripter, scene, time, player } from ""vam-scripter"";

var person = scene.getAtom(""Person"");
var head = person.getController(""headControl"");
var hand = person.getController(""lHandControl"");

scripter.onFixedUpdate(() => {
    head.lookAt(player.head, time.fixedDeltaTime * 30);
});

scripter.onUpdate(() => {
    console.log(""Update: "" + time.time);
});
");
            });
            AddTemplateButton(templates.transform, "Play sounds", () =>
            {
                Scripter.singleton.programFiles.Create(
                    "index.js",
                    @"import { scene } from ""vam-scripter"";

// You can also use your own sounds with ""URL"", ""web"", ""yoursound.wav""
var music = scene.getAudioClipAction(""Embedded"", ""Music"", ""CyberPetrifiedFull"");

// Create an AudioSource atom that will play the music
var speaker = scene.getAtom(""AudioSource"").getStorable(""AudioSource"").getAudioAction(""PlayNow"");

// Play the music
speaker.play(music);
");
            });
            AddTemplateButton(templates.transform, "Documentation (web)\n<color=#6666cc>acidbubbles.github.io</color>",
                () => Application.OpenURL("https://acidbubbles.github.io/vam-scripter/"));
            AddTemplateButton(templates.transform, "Support me (web)\n<color=#6666cc>www.patreon.com</color>",
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
