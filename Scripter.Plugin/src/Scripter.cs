using System.Collections;
using SimpleJSON;
using UnityEngine;

public class Scripter : MVRScript
{
    private readonly ScriptsManager _scripts;
    private ScreenManager _screens;
    private bool _restored;

    public Scripter()
    {
        _scripts = new ScriptsManager();
    }

    public override void Init()
    {
        SuperController.singleton.StartCoroutine(DeferredInit());
    }

    private IEnumerator DeferredInit()
    {
        yield return new WaitForEndOfFrame();
        if (this == null) yield break;
        if (!_restored)
        {
            containingAtom.RestoreFromLast(this);
            _restored = true;
        }
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

    public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true, bool forceStore = false)
    {
        var json = base.GetJSON(includePhysical, includeAppearance, forceStore);
        json["Scripts"] = _scripts.GetJSON();
        needsStore = true;
        return json;
    }

    public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true, JSONArray presetAtoms = null, bool setMissingToDefault = true)
    {
        base.RestoreFromJSON(jc, restorePhysical, restoreAppearance, presetAtoms, setMissingToDefault);
        _scripts.RestoreFromJSON(jc["Scripts"]);
        _restored = true;
    }
}
