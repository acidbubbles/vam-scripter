using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class ScriptsManagerScreen : Screen
{
    public static Screen Create(ScreenManager manager)
    {
        var screen = Screen.Create<ScriptsManagerScreen>(manager.Root);

        var toolbar = MakeToolbar(screen.transform);
        var types = new List<string>
        {
            ScriptActionTrigger.Type,
        };
        var addTypeJSON = new JSONStorableStringChooser("Type", types, types[0], "Trigger:");

        var addTypeUI = Instantiate(manager.Prefabs.configurablePopupPrefab, toolbar).GetComponent<UIDynamicPopup>();
        addTypeUI.GetComponent<LayoutElement>().minHeight = 52;
        addTypeUI.height = 52;
        addTypeJSON.popup = addTypeUI.popup;
        #warning Drop down is hidden!
        AddToToolbar(toolbar, addTypeUI.transform);
        AddToToolbar(toolbar,  CreateButton(toolbar, manager.Prefabs.configurableButtonPrefab, "Create", () => manager.Scripts.Create(addTypeJSON.val)));

        screen.Initialize(manager);

        return screen;
    }

    private readonly List<ScriptEntryComponent> _entries = new List<ScriptEntryComponent>();
    private ScreenManager _manager;

    public void Initialize(ScreenManager manager)
    {
        _manager = manager;
        _manager.Scripts.ScriptsUpdated.AddListener(ScriptsUpdated);
        ScriptsUpdated();
    }

    private void ScriptsUpdated()
    {
        ClearScripts();
        foreach (var script in _manager.Scripts.Scripts)
        {
            _entries.Add(ScriptEntryComponent.Create(transform, _manager, script));
        }
    }

    private void ClearScripts()
    {
        foreach (var entry in _entries)
        {
            DestroyImmediate(entry.gameObject);
        }
        _entries.Clear();
    }

    public void OnDestroy()
    {
        _manager?.Scripts.ScriptsUpdated.RemoveListener(ScriptsUpdated);
    }
}
