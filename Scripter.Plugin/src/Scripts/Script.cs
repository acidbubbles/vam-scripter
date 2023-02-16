using System;
using SimpleJSON;

public class Script
{
    private readonly Scripter _scripter;

    public ScripterTab Tab { get; set; }
    public readonly HistoryManager History;
    public readonly JSONStorableString NameJSON = new JSONStorableString("Module", "");
    public readonly JSONStorableString SourceJSON = new JSONStorableString("Source", "");

    private string _previousName;

    public Script(string moduleName, string source, Scripter scripter)
    {
        _scripter = scripter;

        History = new HistoryManager(SourceJSON);

        _previousName = moduleName;
        NameJSON.val = moduleName;
        NameJSON.setCallbackFunction = val =>
        {
            scripter.ProgramFiles.Program.Remove(_previousName);
            _previousName = val;
        };

        SourceJSON.setCallbackFunction = val =>
        {
            History.Update(val);
            Parse(val);
        };
        SourceJSON.val = source;
    }

    private void Parse(string val)
    {
        #warning Add globals for Init (shared variables)
        try
        {
            _scripter.ProgramFiles.Program.Register(NameJSON.val, val);
            _scripter.Console.Log($"<color=green>{NameJSON.val} parsed successfully; press apply to run.</color>");
        }
        catch (Exception exc)
        {
            _scripter.Console.Log($"<color=red>{NameJSON.val} failed to compile: {exc.Message}</color>");
        }
    }

    public static Script FromJSON(JSONNode json, Scripter plugin)
    {
        var s = new Script(json["Module"].Value, json["Source"].Value, plugin);
        return s;
    }

    public JSONNode GetJSON()
    {
        return new JSONClass
        {
            { "Module", NameJSON.val },
            { "Source", SourceJSON.val },
        };
    }
}
