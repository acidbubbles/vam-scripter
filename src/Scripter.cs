using System;
using UnityEngine;
using UnityEngine.UI;

public class Scripter : MVRScript
{
    private readonly JSONStorableString _scriptJSON;
    private readonly JSONStorableAction _executeScriptJSON;

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
        _scriptJSON.valNoCallback = @"
x = 0;
x++;
log(""The result is: "" + x);
".Trim();

        RegisterString(_scriptJSON);
        RegisterAction(_executeScriptJSON);

        CreateButton("Execute").button.onClick.AddListener(_executeScriptJSON.actionCallback.Invoke);
        CreateTextInput(_scriptJSON);
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

    private static void ProcessScript(string script)
    {
        try
        {
            var result = Interpreter.Instance.Process(script);
        }
        catch (Exception exc)
        {
            SuperController.LogError($"Scripter: There was an error executing the script.\n{exc}");
            ParserFunction.InvalidateStacksAfterLevel(0);
        }
    }
}
