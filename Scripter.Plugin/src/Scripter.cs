using System;
using ScripterLang;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Vam;

public class Scripter : MVRScript
{
    private GlobalLexicalContext _globalLexicalContext;
    private RuntimeDomain _domain;
    private Expression _expression;
    private HistoryManager _history;

    private readonly JSONStorableString _scriptJSON;
    private readonly JSONStorableAction _executeScriptJSON;
    private readonly JSONStorableString _consoleJSON;

    public Scripter()
    {
        _scriptJSON = new JSONStorableString("Script", "");
        _executeScriptJSON = new JSONStorableAction("Execute", ExecuteScript);
        _consoleJSON = new JSONStorableString("Console", "");
    }

    private void ExecuteScript()
    {
        ProcessScript();
    }

    public override void Init()
    {
        _globalLexicalContext = new GlobalLexicalContext();
        VamFunctions.Register(_globalLexicalContext);

        _scriptJSON.valNoCallback = @"
// Welcome to Scripter!
var alpha = getFloatParamValue(""Cube"", ""CubeMat"", ""Alpha Adjust"", 0.5);
if(alpha == 0) {
    logMessage(""The cube is fully transparent"");
} else {
    logMessage(""The cube alpha is: "" + alpha);
}
".Trim();
        _history = new HistoryManager(_scriptJSON);

        _scriptJSON.setCallbackFunction = val =>
        {
            _history.Update(val);
            Parse(_scriptJSON.val);
        };

        RegisterString(_scriptJSON);
        RegisterAction(_executeScriptJSON);

        CreateButton("Execute").button.onClick.AddListener(_executeScriptJSON.actionCallback.Invoke);
        Components.MakeMultilineInput(CreateTextField(_scriptJSON), _scriptJSON);

        var toolbar = Components.MakeToolbar(CreateSpacer());
        Components.AddToToolbar(toolbar, manager.configurableButtonPrefab, "Undo", _history.Undo);
        Components.AddToToolbar(toolbar, manager.configurableButtonPrefab, "Redo", _history.Redo);

        CreateTextField(_consoleJSON);

        Parse(_scriptJSON.val);
    }

    private void Parse(string val)
    {
        try
        {
            _expression = Parser.Parse(val, _globalLexicalContext);
            _domain = new RuntimeDomain(_globalLexicalContext);
            _consoleJSON.val = "<color=green>Code parsed successfully</color>";
        }
        catch (Exception exc)
        {
            _expression = null;
            _domain = null;
            _consoleJSON.val = $"<color=red>Failed to compile.\n{exc}</color>";
        }
    }

    public override void InitUI()
    {
        base.InitUI();
        if (UITransform == null) return;
        leftUIContent.anchorMax = new Vector2(1, 1);
    }

    private void ProcessScript()
    {
        if (_expression == null) return;

        try
        {
            _expression.Evaluate(_domain);
        }
        catch (Exception exc)
        {
            if (!UITransform.gameObject.activeInHierarchy)
                SuperController.LogError($"Scripter: There was an error executing the script.\n{exc.Message}");
            _consoleJSON.val = $"<color=red>{exc}</color>";
        }
    }
}
