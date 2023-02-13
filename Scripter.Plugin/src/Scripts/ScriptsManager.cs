using System;
using System.Collections.Generic;
using System.Linq;
using ScripterLang;
using SimpleJSON;
using UnityEngine.Events;

public class ScriptsManager
{
    private readonly Scripter _plugin;
    public readonly UnityEvent ScriptsUpdated = new UnityEvent();
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

    private string NewName()
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
        }
        ScriptsUpdated.Invoke();
    }

    public Script Create(string filename, string code)
    {
        var script = new Script(filename, code, _plugin);
        Scripts.Add(script);
        return script;
    }
}
