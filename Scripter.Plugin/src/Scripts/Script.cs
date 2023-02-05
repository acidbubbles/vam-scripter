using System;
using ScripterLang;
using SimpleJSON;

public class Script
{
    public readonly HistoryManager History;
    public readonly JSONStorableString SourceJSON = new JSONStorableString("Source", "// Write some code!");
    public readonly JSONStorableString ConsoleJSON = new JSONStorableString("Console", "");
    public ScriptTrigger Trigger;

    private readonly GlobalLexicalContext _globalLexicalContext;
    private RuntimeDomain _domain;
    private Expression _expression;

    public Script(string source = null)
    {
        _globalLexicalContext = new GlobalLexicalContext();
        _globalLexicalContext.StaticDeclarations.Add("value", Value.Undefined);
        VamFunctions.Register(_globalLexicalContext);

        History = new HistoryManager(SourceJSON);

        SourceJSON.setCallbackFunction = val =>
        {
            History.Update(val);
            Parse(val);
        };
        if (source != null)
            SourceJSON.val = source;
        else
            ConsoleJSON.val = "This script is empty.";
    }

    private void Parse(string val)
    {
        try
        {
            _expression = Parser.Parse(val, _globalLexicalContext);
            _domain = new RuntimeDomain(_globalLexicalContext);
            ConsoleJSON.val = "<color=green>Code parsed successfully</color>";
        }
        catch (Exception exc)
        {
            _expression = null;
            _domain = null;
            ConsoleJSON.val = $"<color=red>Failed to compile.\n{exc}</color>";
        }
    }

    public void Run(Value value)
    {
        if (_expression == null) return;

        try
        {
            _domain.SetVariableValue("value", value);
            _expression.Evaluate(_domain);
        }
        catch (Exception exc)
        {
            if (ConsoleJSON.dynamicText == null || !ConsoleJSON.dynamicText.isActiveAndEnabled)
                SuperController.LogError($"Scripter: There was an error executing the script.\n{exc.Message}");
            ConsoleJSON.val = $"<color=red>{exc}</color>";
        }
    }

    public static Script FromJSON(JSONNode json, MVRScript plugin)
    {
        var s = new Script(json["Source"].Value);
        s.Trigger = ScriptTrigger.FromJSON(json["Trigger"], s.Run, plugin);
        return s;
    }

    public JSONNode GetJSON()
    {
        return new JSONClass
        {
            { "Trigger", Trigger.GetJSON() },
            { "Source", SourceJSON.val },
        };
    }
}
