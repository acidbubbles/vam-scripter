using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class ScriptsManagerScreen : Screen
{
    public static Screen Create(ScreenManager manager)
    {
        var screen = Screen.Create<ScriptsManagerScreen>(manager.Root);

        var toolbar = MakeToolbar(screen.transform);
        AddToToolbar(toolbar,  CreateButton(toolbar, manager.Prefabs.configurableButtonPrefab, "Create", manager.Scripts.Add));

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
