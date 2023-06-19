using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            script.tab = _plugin.ui.AddScriptTab(script);
            if(script.nameJSON.val == "index.js")
                _plugin.ui.SelectTab(script.tab);
        }
    }

    public void Create(string filename, string code)
    {
        var script = new Script(filename, code, _plugin);
        files.Add(script);
        script.tab = _plugin.ui.AddScriptTab(script);
        _plugin.ui.SelectTab(script.tab);
    }

    private void Delete(Script script)
    {
        Unregister(script);
        files.Remove(script);
        _plugin.ui.RemoveTab(script.tab);
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
        try
        {
            foreach (var file in files)
            {
                if(file.dirty)
                    file.Parse();
            }

            if (!_program.CanRun())
            {
                return;
            }

            _program.Run();
        }
        catch (Exception exc)
        {
            _plugin.console.LogError($"Failed to run code: {exc}");
        }
    }

    public void RegisterFile(string name, string val)
    {
        _program.RegisterFile(name, val);
    }

    public void Unregister(Script script)
    {
        _program.Unregister(script.nameJSON.val);
    }

    public bool CanRun()
    {
        return _program.CanRun();
    }

    public void ParseAll()
    {
        foreach (var script in files)
        {
            if (!script.dirty) continue;
            script.Parse();
        }
    }
}
