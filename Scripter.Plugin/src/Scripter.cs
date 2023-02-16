using System;
using System.Collections;
using System.Collections.Generic;
using ScripterLang;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;

public class Scripter : MVRScript
{
    public static Scripter Singleton;
    private static readonly Value[] _emptyArgs = new Value[0];

    public readonly ConsoleBuffer Console;
    public readonly ProgramFilesManager ProgramFiles;
    public ScripterUI UI;

    private bool _loading;
    private bool _restored;

    #warning TODO: Update, FixedUpdate, basic MoveTo and LookAt commands, check if an atom exists, list atoms
    #warning TODO: Keybindings
    public Dictionary<string, UnityEvent> KeybindingsTriggers { get; } = new Dictionary<string, UnityEvent>();
    public readonly List<FunctionLink> OnUpdateFunctions = new List<FunctionLink>();

    public Scripter()
    {
        Singleton = this;
        Console = new ConsoleBuffer();
        ProgramFiles = new ProgramFilesManager(this);
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

        if (ProgramFiles.Files.Count == 0)
        {
            UI.SelectTab(UI.AddWelcomeTab());
            Console.Log("> <color=cyan>Welcome to Scripter!</color>");
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
        json["Scripts"] = ProgramFiles.GetJSON();
        needsStore = true;
        return json;
    }

    public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true, JSONArray presetAtoms = null, bool setMissingToDefault = true)
    {
        base.RestoreFromJSON(jc, restorePhysical, restoreAppearance, presetAtoms, setMissingToDefault);
        _loading = true;
        Triggers_RestoreFromJSON(jc["Triggers"]);
        ProgramFiles.RestoreFromJSON(jc["Scripts"]);
        ProgramFiles.Apply();
        _loading = false;
        _restored = true;
        UpdateKeybindings();
    }

    private void Update()
    {
        for (var i = 0; i < OnUpdateFunctions.Count; i++)
        {
            var fn = OnUpdateFunctions[i];
            fn.Fn.Invoke(fn.Context, _emptyArgs);
        }
    }

    #region Triggers Manager

    private readonly List<ScripterParamDeclarationBase> _triggers = new List<ScripterParamDeclarationBase>();

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
            var trigger = ScripterParamDeclarationFactory.FromJSON(triggerJSON);
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
