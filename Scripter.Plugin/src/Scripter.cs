using System.Collections;
using System.Collections.Generic;
using ScripterLang;
using SimpleJSON;
using UnityEngine;

public class Scripter : MVRScript
{
    public static Scripter singleton;

    public readonly ConsoleBuffer console;
    public readonly ProgramFilesManager programFiles;

    public ScripterUI ui;
    public bool isLoading;

    private bool _restored;
    private readonly List<ScripterParamDeclarationBase> _triggers = new List<ScripterParamDeclarationBase>();

    public List<ScripterKeybindingDeclaration> KeybindingsTriggers { get; } = new List<ScripterKeybindingDeclaration>();
    public readonly List<FunctionLink> onUpdateFunctions = new List<FunctionLink>();
    public readonly List<FunctionLink> onFixedUpdateFunctions = new List<FunctionLink>();

    public Scripter()
    {
        singleton = this;
        console = new ConsoleBuffer();
        programFiles = new ProgramFilesManager(this);
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

        if (programFiles.files.Count == 0)
        {
            ui.SelectTab(ui.AddWelcomeTab());
            console.Log("> <color=cyan>Welcome to Scripter!</color>");
        }
        else if (programFiles.CanRun())
        {
            programFiles.Run();
        }
    }

    public override void InitUI()
    {
        base.InitUI();
        if (UITransform == null) return;
        leftUIContent.anchorMax = new Vector2(1, 1);
        ui = ScripterUI.Create(UITransform);
    }

    public override JSONClass GetJSON(bool includePhysical = true, bool includeAppearance = true, bool forceStore = false)
    {
        var json = base.GetJSON(includePhysical, includeAppearance, forceStore);
        var json1 = new JSONClass();
        foreach (var trigger in _triggers)
        {
            json1.Add(trigger.GetJSON());
        }
        json["Triggers"] = json1;
        json["Scripts"] = programFiles.GetJSON();
        needsStore = true;
        return json;
    }

    public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true, JSONArray presetAtoms = null, bool setMissingToDefault = true)
    {
        base.RestoreFromJSON(jc, restorePhysical, restoreAppearance, presetAtoms, setMissingToDefault);
        isLoading = true;
        var array = jc["Triggers"].AsArray;
        if (array == null)
        {
        }
        else
        {
            foreach (JSONNode triggerJSON in array)
            {
                var trigger = ScripterParamDeclarationFactory.FromJSON(triggerJSON);
                _triggers.Add(trigger);
            }
        }

        programFiles.RestoreFromJSON(jc["Scripts"]);
        isLoading = false;
        _restored = true;
        UpdateKeybindings();
    }

    private void Update()
    {
        for (var i = 0; i < onUpdateFunctions.Count; i++)
        {
            var fn = onUpdateFunctions[i];
            fn.fn.Invoke(fn.context, Value.EmptyValues);
        }
    }

    private void FixedUpdate()
    {
        for (var i = 0; i < onFixedUpdateFunctions.Count; i++)
        {
            var fn = onFixedUpdateFunctions[i];
            fn.fn.Invoke(fn.context, Value.EmptyValues);
        }
    }

    public void UpdateKeybindings()
    {
        if(isLoading) return;
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
            bindings.Add(trigger);
        }
    }

    public void OnDestroy()
    {
        SuperController.singleton.BroadcastMessage("OnActionsProviderDestroyed", this, SendMessageOptions.DontRequireReceiver);
    }
}
