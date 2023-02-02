using System;
using System.Collections.Generic;
using ScripterLang;
using UnityEngine;
using UnityEngine.UI;

public class Scripter : MVRScript
{
    private Runtime _runtime;

    private readonly JSONStorableString _scriptJSON;
    private readonly JSONStorableAction _executeScriptJSON;
    private readonly JSONStorableString _consoleJSON;
    private readonly List<string> _history = new List<string>();
    private Expression _expression;

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
        _runtime = new Runtime();

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
            try
            {
                _expression = Parser.Parse(
                    Tokenizer.Tokenize(val).ToList()
                );
                _consoleJSON.val = "Code parsed successfully";
            }
            catch (Exception exc)
            {
                _consoleJSON.val = exc.ToString();
            }
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

    private void ProcessScript()
    {
        try
        {
            var result = _runtime.Evaluate(_expression);
        }
        catch (Exception exc)
        {
            SuperController.LogError($"Scripter: There was an error executing the script.\n{exc.Message}");
            _consoleJSON.val = exc.ToString();
        }
    }
}
