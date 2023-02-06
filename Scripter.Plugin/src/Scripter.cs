using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class Scripter : MVRScript
{
    private readonly ScriptsManager _scripts;
    private ScreenManager _screens;
    private bool _restored;

    public List<ScriptUpdateTrigger> UpdateTriggers { get; } = new List<ScriptUpdateTrigger>();

    public Scripter()
    {
        _scripts = new ScriptsManager(this);
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
            containingAtom.RestoreFromLast(this);
    }

    public override void InitUI()
    {
        base.InitUI();
        if (UITransform == null) return;
        leftUIContent.anchorMax = new Vector2(1, 1);
        _screens = new ScreenManager(UITransform, leftUIContent, manager, _scripts);
        _screens.EditScriptsList();
    }

    public void Update()
    {
        for (var i = 0; i < UpdateTriggers.Count; i++)
        {
            try
            {
                UpdateTriggers[i].Run();
            }
            catch(Exception exc)
            {
                UpdateTriggers[i].EnabledJSON.val = false;
                SuperController.LogError($"Scripter: An Update trigger failed and was disabled: {exc}");
            }
        }
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
