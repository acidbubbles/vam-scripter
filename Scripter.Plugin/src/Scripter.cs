using System.Collections;
using System.Collections.Generic;
using ScripterLang;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;

public class Scripter : MVRScript
{
    public static Scripter Singleton;

    public readonly ScriptsManager Scripts;
    public ScripterUI UI;

    private bool _loading;
    private bool _restored;

    public Dictionary<string, UnityEvent> KeybindingsTriggers { get; } = new Dictionary<string, UnityEvent>();

    public Scripter()
    {
        Scripts = new ScriptsManager(this);
        Singleton = this;
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

        if (Scripts.Scripts.Count == 0)
        {
            UI.SelectTab(UI.AddWelcomeTab());
        }
    }

    public override void InitUI()
    {
        base.InitUI();
        if (UITransform == null) return;
        leftUIContent.anchorMax = new Vector2(1, 1);
        UI = ScripterUI.Create(UITransform, this);
    }

    public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true, bool forceStore = false)
    {
        var json = base.GetJSON(includePhysical, includeAppearance, forceStore);
        json["Triggers"] = Triggers_GetJSON();
        json["Scripts"] = Scripts.GetJSON();
        needsStore = true;
        return json;
    }

    public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true, JSONArray presetAtoms = null, bool setMissingToDefault = true)
    {
        base.RestoreFromJSON(jc, restorePhysical, restoreAppearance, presetAtoms, setMissingToDefault);
        _loading = true;
        Triggers_RestoreFromJSON(jc["Triggers"]);
        Scripts.RestoreFromJSON(jc["Scripts"]);
        Scripts.Apply();
        _loading = false;
        _restored = true;
        UpdateKeybindings();
    }

    #region Triggers Manager

    private readonly List<ScripterParamBase> _triggers = new List<ScripterParamBase>();

    public JSONNode Triggers_GetJSON()
    {
        var json = new JSONClass();
        foreach (var trigger in _triggers)
        {
            json.Add(trigger.GetJSON());
        }
        return json;
    }

    public void Triggers_RestoreFromJSON(JSONNode json)
    {
        var array = json.AsArray;
        if (array == null) return;
        foreach (JSONNode triggerJSON in array)
        {
            var trigger = ScripterParamFactory.FromJSON(triggerJSON);
            _triggers.Add(trigger);
        }
    }

    #endregion

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
