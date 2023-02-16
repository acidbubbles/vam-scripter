using System;
using System.Collections.Generic;
using System.Linq;
using ScripterLang;
using SimpleJSON;

public class ProgramFilesManager
{
    private readonly Scripter _plugin;

    public readonly List<Script> Files = new List<Script>();
    public readonly Program Program;

    public ProgramFilesManager(Scripter plugin)
    {
        _plugin = plugin;
        Program = new Program();
        GlobalFunctions.Register(Program.GlobalContext);
    }

    public string NewName()
    {
        const string prefix = "lib";
        for (var i = 1; i < 9999; i++)
        {
            var name = $"{prefix}{i}.js";
            if (Files.All(s => s.NameJSON.val != name))
                return name;
        }
        throw new InvalidOperationException("You're creating way too many scripts!");
    }

    public JSONNode GetJSON()
    {
        var json = new JSONArray();
        foreach (var script in Files)
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
            Files.Add(script);
            script.Tab = _plugin.UI.AddScriptTab(script);
            if(script.NameJSON.val == "index.js")
                _plugin.UI.SelectTab(script.Tab);
        }
    }

    public Script Create(string filename, string code)
    {
        var script = new Script(filename, code, _plugin);
        Files.Add(script);
        script.Tab = _plugin.UI.AddScriptTab(script);
        _plugin.UI.SelectTab(script.Tab);
        return script;
    }

    public void Unregister(Script script)
    {
        Program.Unregister(script.NameJSON.val);
        Files.Remove(script);
        _plugin.UI.RemoveTab(script.Tab);
        #warning Unregister listeners?
    }

    public void Clear()
    {
        foreach (var script in Files.ToArray())
        {
            Unregister(script);
        }
    }

    public void Apply()
    {
        try
        {
            Program.Run();
            _plugin.Console.Log("index.js is now live");
        }
        catch (Exception exc)
        {
            _plugin.Console.LogError($"Failed to run code: {exc.Message}");
        }
    }
}
