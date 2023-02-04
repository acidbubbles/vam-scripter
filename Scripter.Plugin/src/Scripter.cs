using UnityEngine;
using UnityEngine.UI;

public class Scripter : MVRScript
{
    private readonly ScriptsManager _scripts;
    private ScreenManager _screens;

    public Scripter()
    {
        _scripts = new ScriptsManager();
    }

    public override void Init()
    {
        _scripts.Add("1");
    }

    public override void InitUI()
    {
        base.InitUI();
        if (UITransform == null) return;
        leftUIContent.anchorMax = new Vector2(1, 1);
        _screens = new ScreenManager(UITransform, leftUIContent, manager, _scripts);
        SuperController.singleton.transform.parent.BroadcastMessage("DevToolsGameObjectExplorerShow", UITransform.gameObject);
        _screens.EditScriptsList();
    }
}
