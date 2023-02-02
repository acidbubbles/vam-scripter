using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scripter : MVRScript
{
    private Interpreter _interpreter;

    private readonly JSONStorableString _scriptJSON;
    private readonly JSONStorableAction _executeScriptJSON;
    private readonly List<string> _history = new List<string>();

    public Scripter()
    {
        _scriptJSON = new JSONStorableString("Script", "");
        _executeScriptJSON = new JSONStorableAction("Execute", ExecuteScript);
    }

    private void ExecuteScript()
    {
        ProcessScript(_scriptJSON.val);
    }

    public override void Init()
    {
        _interpreter = new Interpreter();
        ParserFunction.AddGlobal(LogMessageFunction.FunctionName, new LogMessageFunction());

        _scriptJSON.valNoCallback = @"
// Welcome to Scripter!
x = 0;
x++;
if(x == 0) {
    logMessage(""The result is: "" + x);
}
".Trim();
        _history.Add(_scriptJSON.val);

        _scriptJSON.setCallbackFunction = val =>
        {
            _history.Add(val);
            SuperController.LogMessage("History: " + _history.Count);
            if (_history.Count > 100) _history.RemoveAt(0);
        };

        RegisterString(_scriptJSON);
        RegisterAction(_executeScriptJSON);

        CreateButton("Execute").button.onClick.AddListener(_executeScriptJSON.actionCallback.Invoke);
        CreateTextInput(_scriptJSON);

        // TODO: Toolbar
        CreateButton("Undo").button.onClick.AddListener(Undo);
    }

    private void Undo()
    {
        // TODO: Improve this (undo should not delete history, just go back)
        if (_history.Count == 0) return;
        SuperController.LogMessage("Undo " + _history.Count);
        _scriptJSON.valNoCallback = _history[_history.Count - 1];
        _history.RemoveAt(_history.Count - 1);
    }

    public override void InitUI()
    {
        base.InitUI();
        if (UITransform == null) return;
        leftUIContent.anchorMax = new Vector2(1, 1);
    }

    private UIDynamicTextField CreateTextInput(JSONStorableString jss)
    {
        var textfield = CreateTextField(jss);
        textfield.height = 700;
        jss.dynamicText = textfield;
        textfield.backgroundColor = Color.white;
        var text = textfield.GetComponentInChildren<Text>(true);
        var input = text.gameObject.AddComponent<InputField>();
        input.lineType = InputField.LineType.MultiLineNewline;
        input.textComponent = textfield.UItext;
        jss.inputField = input;
        return textfield;
    }

    private void ProcessScript(string script)
    {
        try
        {
            var result = _interpreter.Process(script);
        }
        catch (Exception exc)
        {
            SuperController.LogError($"Scripter: There was an error executing the script.\n{exc.Message}");
            ParserFunction.InvalidateStacksAfterLevel(0);
        }
    }
}
