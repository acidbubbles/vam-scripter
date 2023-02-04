using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScreenManager
{
    private readonly Transform _ui;
    public readonly Transform Root;
    public readonly MVRPluginManager Prefabs;
    private readonly Button _close;
    public readonly ScriptsManager Scripts;
    private Screen _screen;

    public ScreenManager(Transform ui, Transform root, MVRPluginManager manager, ScriptsManager scripts)
    {
        _ui = ui;
        Root = root;
        Prefabs = manager;
        Scripts = scripts;
        _close = ui.Find("CloseButton").GetComponent<Button>();
    }

    public void EditScriptsList()
    {
        CloseScreen();
        _screen = ScriptsManagerScreen.Create(this);
        ChangeCloseBehavior(() => _ui.gameObject.SetActive(false));
    }

    public void EditScript(Script script)
    {
        CloseScreen();
        _screen = ScriptEditScreen.Create(this, script);
        ChangeCloseBehavior(EditScriptsList);
    }

    private void ChangeCloseBehavior(UnityAction action)
    {
        _close.onClick.RemoveAllListeners();
        _close.onClick.AddListener(action);
    }

    private void CloseScreen()
    {
        if (_screen != null)
            Object.DestroyImmediate(_screen.gameObject);
    }
}
