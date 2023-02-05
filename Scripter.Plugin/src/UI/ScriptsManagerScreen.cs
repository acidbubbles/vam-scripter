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

        var toolbar = MakeToolbar(screen.transform, 56);
        var types = new List<string>
        {
            ScriptActionTrigger.Type,
            ScriptFloatParamTrigger.Type,
            ScriptBoolParamTrigger.Type,
            ScriptUpdateTrigger.Type,
        };
        var addTypeJSON = new JSONStorableStringChooser("Type", types, types[0], "Add script:");

        var addTypeUI = Instantiate(manager.Prefabs.configurablePopupPrefab, toolbar).GetComponent<UIDynamicPopup>();
        screen._addTypePopup = addTypeUI.popup;
        addTypeUI.GetComponent<LayoutElement>().minHeight = 60;
        addTypeUI.height = 60;
        addTypeJSON.popup = addTypeUI.popup;
        addTypeUI.popup.topButton.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        AddToToolbar(toolbar, addTypeUI.transform);
        var buttonUI = CreateButton(toolbar, manager.Prefabs.configurableButtonPrefab, "+ Create", () => manager.Scripts.Create(addTypeJSON.val));
        AddToToolbar(toolbar,  buttonUI);

        screen.Initialize(manager);

        return screen;
    }

    private readonly List<ScriptEntryComponent> _entries = new List<ScriptEntryComponent>();
    private ScreenManager _manager;
    private UIPopup _addTypePopup;

    public void Initialize(ScreenManager manager)
    {
        _manager = manager;
        _manager.Scripts.ScriptsUpdated.AddListener(ScriptsUpdated);
        ScriptsUpdated();

        _addTypePopup.onOpenPopupHandlers += OnOpenPopupHandler;
    }

    private void OnOpenPopupHandler()
    {
        Invoke(nameof(MovePopupToTop), 0);
    }

    private void MovePopupToTop()
    {
        _addTypePopup.popupPanel.SetParent(transform, true);
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
        if (_addTypePopup != null) _addTypePopup.onOpenPopupHandlers -= OnOpenPopupHandler;
    }
}
