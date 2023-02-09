using System;
using ScripterLang;
using SimpleJSON;

public class Script
{
    public readonly HistoryManager History;
    public readonly JSONStorableString SourceJSON = new JSONStorableString("Source", "// Write some code!");
    public readonly JSONStorableString ConsoleJSON = new JSONStorableString("Console", "");
    public ScriptTrigger Trigger;

    private readonly Program _program = new Program();
    private Expression _expression;

    public Script(string source = null)
    {
        GlobalFunctions.Register(_program.GlobalContext);

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
        #warning Add globals for Init (shared variables)
        try
        {
            _program.Add("", val);
            ConsoleJSON.val = "<color=green>Code parsed successfully</color>";
        }
        catch (Exception exc)
        {
            _expression = null;
            ConsoleJSON.val = $"<color=red>Failed to compile.\n{exc}</color>";
        }
    }

    public void Run(Value value)
    {
        if (_expression == null) return;

        try
        {
            #warning Change to onEvent
            // if (value.Type != ValueTypes.Uninitialized)
            //     _domain.Variables["value"] = value;
            _expression.Evaluate();
        }
        catch (Exception exc)
        {
            if (ConsoleJSON.dynamicText == null || !ConsoleJSON.dynamicText.isActiveAndEnabled)
                SuperController.LogError($"Scripter: There was an error executing the script.\n{exc.Message}");
            ConsoleJSON.val = $"<color=red>{exc}</color>";
        }
    }

    public static Script FromJSON(JSONNode json, Scripter plugin)
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
