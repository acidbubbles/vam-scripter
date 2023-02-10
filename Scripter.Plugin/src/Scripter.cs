using System.Collections;
using System.Collections.Generic;
using ScripterLang;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;

public class Scripter : MVRScript
{
    public readonly ScriptsManager Scripts;

    private bool _loading;
    private bool _restored;

    public UnityEvent OnUpdate = new UnityEvent();
    public UnityEvent OnSceneLoaded = new UnityEvent();
    public Dictionary<string, UnityEvent> KeybindingsTriggers { get; } = new Dictionary<string, UnityEvent>();

    public Scripter()
    {
        Scripts = new ScriptsManager(this);
    }

    public override void Init()
    {
        RegisterAction(new JSONStorableAction("Run Performance Test", PerfTest.Run));
        SuperController.singleton.StartCoroutine(DeferredInit());
    }

    private IEnumerator DeferredInit()
    {
        yield return new WaitForEndOfFrame();
        if (this == null) yield break;
        if (!_restored)
            containingAtom.RestoreFromLast(this);

        OnSceneLoaded.Invoke();

        if (!_restored)
            Scripts.CreateIndex();
    }

    public override void InitUI()
    {
        base.InitUI();
        if (UITransform == null) return;
        leftUIContent.anchorMax = new Vector2(1, 1);
        ScripterUI.Create(leftUIContent, this);
    }

    public void Update()
    {
        OnUpdate.Invoke();
    }

    public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true, bool forceStore = false)
    {
        var json = base.GetJSON(includePhysical, includeAppearance, forceStore);
        json["Scripts"] = Scripts.GetJSON();
        needsStore = true;
        return json;
    }

    public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true, JSONArray presetAtoms = null, bool setMissingToDefault = true)
    {
        base.RestoreFromJSON(jc, restorePhysical, restoreAppearance, presetAtoms, setMissingToDefault);
        _loading = true;
        Scripts.RestoreFromJSON(jc["Scripts"]);
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
            var n = trigger.Key;
            if (n == "") continue;
            bindings.Add(new JSONStorableAction(n, trigger.Value.Invoke));
        }
    }

    public void OnDestroy()
    {
        SuperController.singleton.BroadcastMessage("OnActionsProviderDestroyed", this, SendMessageOptions.DontRequireReceiver);
    }
}
