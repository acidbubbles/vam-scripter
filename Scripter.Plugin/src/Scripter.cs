using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MVR.FileManagementSecure;
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

    public readonly List<KeybindingDeclaration> keybindingsTriggers = new List<KeybindingDeclaration>();
    public readonly List<FunctionLink> onUpdateFunctions = new List<FunctionLink>();
    public readonly List<FunctionLink> onLateUpdateFunctions = new List<FunctionLink>();
    public readonly List<FunctionLink> onFixedUpdateFunctions = new List<FunctionLink>();
    public readonly List<FunctionLink> onEnableFunctions = new List<FunctionLink>();
    public readonly List<FunctionLink> onDisableFunctions = new List<FunctionLink>();
    public readonly List<FunctionLink> onDestroyFunctions = new List<FunctionLink>();

    private bool _restored;
    private string _syncFolder;
    private readonly List<ParamDeclarationBase> _triggers = new List<ParamDeclarationBase>();

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

    [MethodImpl(0x0100)]
    private void InvokeCallback(IList<FunctionLink> funcs)
    {
        for (var i = 0; i < funcs.Count; i++)
        {
            var fn = funcs[i];
            try
            {
                fn.fn.Invoke(fn.context, Value.EmptyValues);
            }
            catch (Exception exc)
            {
                console.LogError($"Failed to run {fn.name} callback (will be disabled): {exc.Message}");
                fn.Dispose();
                return;
            }
        }
    }

    private IEnumerator DeferredInit()
    {
        yield return new WaitForEndOfFrame();
        if (this == null) yield break;

        if (IsSessionPlugin())
        {
            _syncFolder = $"Saves\\PluginData\\Scripter\\Session\\{name}";
            LoadFromDisk(false);
        }
        else if (!_restored)
        {
            containingAtom.RestoreFromLast(this);
        }

        if (programFiles.files.Count == 0)
        {
            ui.SelectTab(ui.AddWelcomeTab());
            console.Log("> <color=cyan>Welcome to Scripter!</color>");
        }
        else
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
        if (IsSessionPlugin() || _syncFolder != null) return json;
        var triggersJSON = new JSONClass();
        foreach (var trigger in _triggers)
        {
            triggersJSON.Add(trigger.GetJSON());
        }
        json["Triggers"] = triggersJSON;
        json["Scripts"] = programFiles.GetJSON();
        needsStore = true;
        return json;
    }

    public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = true, bool restoreAppearance = true, JSONArray presetAtoms = null, bool setMissingToDefault = true)
    {
        base.RestoreFromJSON(jc, restorePhysical, restoreAppearance, presetAtoms, setMissingToDefault);

        if (IsSessionPlugin() || _syncFolder != null) return;
        isLoading = true;
        var array = jc["Triggers"].AsArray;
        if (array != null)
        {
            foreach (JSONNode triggerJSON in array)
            {
                var trigger = DeclarationFactory.FromJSON(triggerJSON);
                _triggers.Add(trigger);
            }
        }

        programFiles.RestoreFromJSON(jc["Scripts"]);

        isLoading = false;
        _restored = true;
        UpdateKeybindings();
    }

    public bool IsSessionPlugin()
    {
        return containingAtom.type == "SessionPluginManager";
    }

    private void Update()
    {
       InvokeCallback(onUpdateFunctions);
    }

    private void LateUpdate()
    {
        InvokeCallback(onLateUpdateFunctions);
    }

    private void FixedUpdate()
    {
        InvokeCallback(onFixedUpdateFunctions);
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

        bindings.Add(new JSONStorableAction("OpenUI", SelectAndOpenUI));

        foreach (var trigger in keybindingsTriggers)
        {
            if (trigger.actionJSON == null) throw new NullReferenceException("Null keybindings action JSON");
            bindings.Add(trigger.actionJSON);
        }
    }

    #region Show UI

    private void SelectAndOpenUI()
    {
        if (containingAtom == null) return;
        if (UITransform != null && UITransform.gameObject.activeInHierarchy) return;
        if (SuperController.singleton.gameMode != SuperController.GameMode.Edit)
            SuperController.singleton.gameMode = SuperController.GameMode.Edit;

        if (IsSessionPlugin())
        {
            SuperController.singleton.activeUI = SuperController.ActiveUI.MainMenu;
            SuperController.singleton.SetMainMenuTab("TabSessionPlugins");
            OpenThisCustomUI();
        }
        else if (containingAtom.type == "ScenePluginManager")
        {
            SuperController.singleton.activeUI = SuperController.ActiveUI.MainMenu;
            SuperController.singleton.SetMainMenuTab("TabScenePlugins");
            OpenThisCustomUI();
        }
        else
        {
#if (VAM_GT_1_20)
            SuperController.singleton.SelectController(containingAtom.mainController, false, false, true);
#else
            SuperController.singleton.SelectController(containingAtom.mainController);
#endif
            SuperController.singleton.ShowMainHUDAuto();
            StartCoroutine(WaitForUI());
        }
    }

    private IEnumerator WaitForUI()
    {
        var expiration = Time.unscaledTime + 1f;
        while (Time.unscaledTime < expiration)
        {
            yield return 0;
            var selector = containingAtom.gameObject.GetComponentInChildren<UITabSelector>();
            if (selector == null) continue;
            selector.SetActiveTab("Plugins");
            // ReSharper disable once RedundantJumpStatement
            if (UITransform == null) continue;
        }

        if (UITransform.gameObject.activeSelf) yield break;

        OpenThisCustomUI();
    }

    private void OpenThisCustomUI()
    {
        foreach (Transform scriptController in manager.pluginContainer)
        {
            var script = scriptController.gameObject.GetComponent<MVRScript>();
            if (script != null && script != this)
            {
                script.UITransform.gameObject.SetActive(false);
            }
        }

        UITransform.gameObject.SetActive(true);
    }

    #endregion

    public void OnEnable()
    {
        InvokeCallback(onEnableFunctions);
    }

    public void OnDisable()
    {
        InvokeCallback(onDisableFunctions);
    }

    public void OnDestroy()
    {
        SuperController.singleton.BroadcastMessage("OnActionsProviderDestroyed", this, SendMessageOptions.DontRequireReceiver);
        InvokeCallback(onDestroyFunctions);
    }

    #region Sync

    private void OnApplicationFocus(bool hasFocus)
    {
        LoadFromDisk(true);
    }

    private void LoadFromDisk(bool tryRun)
    {
        if (_syncFolder == null) return;
        if(!FileManagerSecure.DirectoryExists(_syncFolder))
            FileManagerSecure.CreateDirectory(_syncFolder);
        var paths = FileManagerSecure.GetFiles(_syncFolder);
        var changed = false;
        foreach (var path in paths)
        {
            var localName = path.Split('\\').Last();
            var script = programFiles.files.FirstOrDefault(f => f.nameJSON.val == localName);
            var contents = FileManagerSecure.ReadAllText(path);
            if (script != null)
            {
                if (contents == script.sourceJSON.val) continue;
                if (script.input != null) script.input.enhancementsEnabled = false;
                try
                {
                    script.sourceJSON.val = contents;
                }
                finally
                {
                    if (script.input != null) script.input.enhancementsEnabled = true;
                }

                changed = true;
            }
            else
            {
                programFiles.Create(localName, contents);
                changed = true;
            }
        }

        if (tryRun && changed)
            programFiles.Run();
    }

    public void SaveToDisk(Script script)
    {
        if (containingAtom.type != "SessionPluginManager") return;
        var localName = script.nameJSON.val;
        if (localName.Contains("/") || localName.Contains("\\")) throw new InvalidOperationException("Unexpected script name: " + script.nameJSON.val);
        FileManagerSecure.WriteAllText($"{_syncFolder}\\{localName}", script.sourceJSON.val);
    }

    #endregion

}
