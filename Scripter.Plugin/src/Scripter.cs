#define SCRIPTER_RUN_PERF_TEST
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
#if SCRIPTER_RUN_PERF_TEST
using ScripterLang;
#endif

public class Scripter : MVRScript
{
    private readonly ScriptsManager _scripts;
    private ScreenManager _screens;
    private bool _loading;
    private bool _restored;

    public List<ScriptUpdateTrigger> UpdateTriggers { get; } = new List<ScriptUpdateTrigger>();
    public List<ScriptLoadTrigger> LoadTriggers { get; } = new List<ScriptLoadTrigger>();
    public List<ScriptKeybindingsTrigger> KeybindingsTriggers { get; } = new List<ScriptKeybindingsTrigger>();

    public Scripter()
    {
        _scripts = new ScriptsManager(this);
    }

    public override void Init()
    {
        #if(SCRIPTER_RUN_PERF_TEST)
        PerformanceTest();
        #endif
        SuperController.singleton.StartCoroutine(DeferredInit());
    }

    private IEnumerator DeferredInit()
    {
        yield return new WaitForEndOfFrame();
        if (this == null) yield break;
        if (!_restored)
            containingAtom.RestoreFromLast(this);

        foreach (var trigger in LoadTriggers)
            trigger.Run();
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
        _loading = true;
        _scripts.RestoreFromJSON(jc["Scripts"]);
        _loading = false;
        _restored = true;
        UpdateKeybindings();
    }

    public void UpdateKeybindings()
    {
        if(_loading) return;
        SuperController.singleton.BroadcastMessage("OnActionsProviderAvailable", this, SendMessageOptions.DontRequireReceiver);
    }

    public void OnBindingsListRequested(List<object> bindings)
    {
        bindings.Add(new Dictionary<string, string>
        {
            {"Namespace", "Scripter"}
        });

        foreach (var trigger in KeybindingsTriggers)
        {
            var n = trigger.NameJSON.val;
            if (n == "") continue;
            bindings.Add(new JSONStorableAction(n, trigger.Run));
        }
    }

    public void OnDestroy()
    {
        SuperController.singleton.BroadcastMessage("OnActionsProviderDestroyed", this, SendMessageOptions.DontRequireReceiver);
    }

    #if SCRIPTER_RUN_PERF_TEST
    private void PerformanceTest()
    {
        const string code = @"
var x1 = 0;
function test(x2) {
    for(var i = 0; i < 100; i++) {
        x2++;
    }
    return x2;
}
{
    for(var j = 0; j < 100; j++) {
        x1 = test(x1);
    }
}
";
        var script = new Script(code);
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        for (var i = 0; i < 100; i++)
            script.Run(Value.Undefined);
        sw.Stop();
        SuperController.singleton.ClearMessages();
        SuperController.LogMessage(sw.Elapsed.TotalSeconds.ToString("0.0000"));
    }
    #endif
}
