using UnityEngine;

public class Scripter : MVRScript
{
    private readonly ScriptsManager _scripts;
    private Screen _screen;

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
        _screen = ScriptEditScreen.Create(leftUIContent, manager, _scripts.ByName("1"));
    }
}
