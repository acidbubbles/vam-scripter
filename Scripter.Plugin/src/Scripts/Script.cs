using System;
using SimpleJSON;
using UnityEngine;

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
            scripter.ProgramFiles.Unregister(this);
            _previousName = val;
            Parse();
        };

        SourceJSON.setCallbackFunction = val =>
        {
            History.Update(val);
        };
        SourceJSON.valNoCallback = source;
        if (!string.IsNullOrEmpty(source))
            Parse();
    }

    private int _lastParsed;
    public void Parse()
    {
        // To prevent clicking on Validate running the script twice
        if (_lastParsed == Time.frameCount) return;
        _lastParsed = Time.frameCount;
        Parse(SourceJSON.val);
    }

    private void Parse(string val)
    {
        try
        {
            _scripter.ProgramFiles.Register(NameJSON.val, val);
            if (_scripter.IsLoading) return;
            var canRun = _scripter.ProgramFiles.CanRun();
            _scripter.Console.Log($"<color=green>Parsed `{NameJSON.val}` successfully; {(canRun ? "Running" : "Waiting for index.js")}.</color>");
            if (canRun)
                _scripter.ProgramFiles.Run();
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
