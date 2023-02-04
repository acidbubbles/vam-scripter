using System;
using ScripterLang;
using SimpleJSON;

public class Script
{

    public readonly HistoryManager History;
    public readonly JSONStorableString NameJSON = new JSONStorableString("Name", "");
    public readonly JSONStorableString SourceJSON = new JSONStorableString("Source", "// Write some code!");
    public readonly JSONStorableString ConsoleJSON = new JSONStorableString("Console", "");

    private readonly GlobalLexicalContext _globalLexicalContext;
    private RuntimeDomain _domain;
    private Expression _expression;

    public JSONStorableAction TriggerJSON;

    public Script(string name, string source = null)
    {
        _globalLexicalContext = new GlobalLexicalContext();
        VamFunctions.Register(_globalLexicalContext);

        History = new HistoryManager(SourceJSON);
        TriggerJSON = new JSONStorableAction("Run", Run);

        NameJSON.valNoCallback = name;
        SourceJSON.setCallbackFunction = val =>
        {
            History.Update(val);
            Parse(val);
        };
        if (source != null)
            SourceJSON.val = source;
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

    public void Run()
    {
        if (_expression == null) return;

        try
        {
            _expression.Evaluate(_domain);
        }
        catch (Exception exc)
        {
            if (ConsoleJSON.dynamicText == null || !ConsoleJSON.dynamicText.isActiveAndEnabled)
                SuperController.LogError($"Scripter: There was an error executing the script.\n{exc.Message}");
            ConsoleJSON.val = $"<color=red>{exc}</color>";
        }
    }

    public JSONNode GetJSON()
    {
        var json = new JSONClass
        {
            { "Name", NameJSON.val },
            { "Source", NameJSON.val },
        };
        return json;
    }

    public static Script FromJSON(JSONNode json)
    {
        var script = new Script(json["Name"], json["Source"]);
        return script;
    }
}
