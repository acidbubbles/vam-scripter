using System;
using System.Collections.Generic;
using System.Linq;
using ScripterLang;
using SimpleJSON;

public class ScriptsManager
{
    private readonly Scripter _plugin;

    public readonly List<Script> Scripts = new List<Script>();
    public readonly Program Program;

    public readonly JSONStorableString ConsoleJSON = new JSONStorableString("Console", "");

    public ScriptsManager(Scripter plugin)
    {
        _plugin = plugin;
        Program = new Program();
        GlobalFunctions.Register(Program.GlobalContext);
        ConsoleJSON.valNoCallback = "> <color=cyan>Welcome to Scripter!</color>";
    }

    public string NewName()
    {
        const string prefix = "lib";
        for (var i = 1; i < 9999; i++)
        {
            var name = $"{prefix}{i}.js";
            if (Scripts.All(s => s.NameJSON.val != name))
                return name;
        }
        throw new InvalidOperationException("You're creating way too many scripts!");
    }

    public JSONNode GetJSON()
    {
        var json = new JSONArray();
        foreach (var script in Scripts)
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
            Scripts.Add(script);
            script.Tab = _plugin.UI.AddScriptTab(script);
            if(script.NameJSON.val == "index.js")
                _plugin.UI.SelectTab(script.Tab);
        }
    }

    public Script Create(string filename, string code)
    {
        var script = new Script(filename, code, _plugin);
        Scripts.Add(script);
        script.Tab = _plugin.UI.AddScriptTab(script);
        _plugin.UI.SelectTab(script.Tab);
        return script;
    }

    public void Remove(Script script)
    {
        Scripts.Remove(script);
        _plugin.UI.RemoveTab(script.Tab);
        #warning Unregister listeners?
    }

    public void Clear()
    {
    }

    public void Apply()
    {
        try
        {
            Program.Run();
            Log("index.js is now live");
        }
        catch (Exception exc)
        {
            LogError($"Failed to run code: {exc.Message}");
        }
    }

    public void Log(string message)
    {
        #warning Optimize
        ConsoleJSON.val += "\n" + message;
        #warning Auto scroll the text
    }

    public void LogError(string message)
    {
        #warning Optimize
        ConsoleJSON.val += "\n<color=red>" + message + "</color>";
        if (!Scripter.Singleton.Scripts.ConsoleJSON.text.isActiveAndEnabled)
        {
            SuperController.LogError("Scripter: " + message);
        }
    }
}
