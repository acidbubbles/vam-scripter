using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class CreateView : MonoBehaviour
{
    public static CreateView Create(Transform parent, ScripterUI ui)
    {
        var go = new GameObject();
        go.transform.SetParent(parent, false);
        go.SetActive(false);

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

        var grid = go.AddComponent<VerticalLayoutGroup>();
        grid.spacing = 10;
        grid.childControlHeight = true;
        grid.childForceExpandHeight = false;
        grid.childAlignment = TextAnchor.UpperCenter;
        grid.padding = new RectOffset(30, 30, 30, 30);

        var view = go.AddComponent<CreateView>();

        view._createIndex = AddButton(go.transform, "Create index (main) script", () =>
        {
            Scripter.Singleton.ProgramFiles.Create("index.js", "import { self } from \"scripter\";\n\n// Write your code here!");
        });

        AddButton(go.transform, "Create new library script", () =>
        {
            var name = Scripter.Singleton.ProgramFiles.NewName();
            Scripter.Singleton.ProgramFiles.Create(name, "export function myFunction() {\n  // Write your code here!\n}");
        });

        return view;
    }

    private UIDynamicButton _createIndex;

    private static UIDynamicButton AddButton(Transform parent, string label, UnityAction action)
    {
        var button = Instantiate(Scripter.Singleton.manager.configurableButtonPrefab, parent, false);

        var ui = button.GetComponent<UIDynamicButton>();

        ui.button.onClick.AddListener(action);

        ui.label = label;

        return ui;
    }

    private void OnEnable()
    {
        _createIndex.button.interactable = Scripter.Singleton.ProgramFiles.Files.All(s => s.NameJSON.val != "index.js");
    }
}
