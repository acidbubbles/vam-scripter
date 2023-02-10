using System;
using SimpleJSON;
using UnityEngine.Events;

public class LogEvent : UnityEvent<ScriptLog>
{
}

public struct ScriptLog
{
    public bool Error;
    public string Message;
}

public class Script
{
    private readonly Scripter _scripter;
    public readonly HistoryManager History;
    public readonly JSONStorableString NameJSON = new JSONStorableString("Module", "");
    public readonly JSONStorableString SourceJSON = new JSONStorableString("Source", "");
    public readonly LogEvent Log = new LogEvent();

    private string _previousName;

    public Script(string moduleName, string source, Scripter scripter)
    {
        _scripter = scripter;

        History = new HistoryManager(SourceJSON);

        _previousName = moduleName;
        NameJSON.val = moduleName;
        NameJSON.setCallbackFunction = val =>
        {
            scripter.Scripts.Program.Remove(_previousName);
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
            _scripter.Scripts.Program.Add(NameJSON.val, val);
            Log.Invoke(new ScriptLog
            {
                Error = false,
                Message = "<color=green>Code parsed successfully</color>"
            });
        }
        catch (Exception exc)
        {
            Log.Invoke(new ScriptLog
            {
                Error = false,
                Message = $"<color=red>Failed to compile.\n{exc}</color>"
            });
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
