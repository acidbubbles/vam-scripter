using System;
using System.Collections.Generic;
using System.Linq;
using ScripterLang;
using SimpleJSON;

public class ProgramFilesManager
{
    private readonly Scripter _plugin;
    private readonly Program _program;

    public readonly List<Script> files = new List<Script>();

    public ProgramFilesManager(Scripter plugin)
    {
        _plugin = plugin;
        _program = new Program();
        Globals.Register(_program.globalContext);
    }

    public string NewName()
    {
        const string prefix = "lib";
        for (var i = 1; i < 9999; i++)
        {
            var name = $"{prefix}{i}.js";
            if (files.All(s => s.nameJSON.val != name))
                return name;
        }
        throw new InvalidOperationException("You're creating way too many scripts!");
    }

    public JSONNode GetJSON()
    {
        var json = new JSONArray();
        foreach (var script in files)
        {
            json.Add(script.GetJSON());
        }
        return json;
    }

    public void RestoreFromJSON(JSONNode json)
    {
        var array = json.AsArray;
        foreach (JSONNode scriptJSON in array)
        {
            var script = Script.FromJSON(scriptJSON, _plugin);
            files.Add(script);
            script.Tab = _plugin.ui.AddScriptTab(script);
            if(script.nameJSON.val == "index.js")
                _plugin.ui.SelectTab(script.Tab);
        }
    }

    public void Create(string filename, string code)
    {
        var script = new Script(filename, code, _plugin);
        files.Add(script);
        script.Tab = _plugin.ui.AddScriptTab(script);
        _plugin.ui.SelectTab(script.Tab);
    }

    private void Delete(Script script)
    {
        Unregister(script);
        files.Remove(script);
        _plugin.ui.RemoveTab(script.Tab);
    }

    public void DeleteAll()
    {
        foreach (var script in files.ToArray())
        {
            Delete(script);
        }
    }

    public void Run()
    {
        if (!_program.CanRun())
        {
            return;
        }

        try
        {
            _program.Run();
        }
        catch (Exception exc)
        {
            _plugin.console.LogError($"Failed to run code: {exc.Message}");
        }
    }

    public void Register(string name, string val)
    {
        _program.Register(name, val);
    }

    public void Unregister(Script script)
    {
        _program.Unregister(script.nameJSON.val);
    }

    public bool CanRun()
    {
        return _program.CanRun();
    }
}
