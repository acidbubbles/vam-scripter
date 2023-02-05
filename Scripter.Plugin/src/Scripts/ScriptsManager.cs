using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine.Events;

public class ScriptsManager
{
    private readonly MVRScript _plugin;
    public readonly UnityEvent ScriptsUpdated = new UnityEvent();
    public readonly List<Script> Scripts = new List<Script>();

    public ScriptsManager(MVRScript plugin)
    {
        _plugin = plugin;
    }

    public void Create(string type)
    {
        var name = NewName();
        var script = new Script();
        script.Trigger = ScriptTrigger.Create(type, name, script.Run, _plugin);
        Scripts.Add(script);
        script.Trigger.Register();
        ScriptsUpdated.Invoke();
    }

    public void Delete(Script script)
    {
        script.Trigger.Deregister();
        Scripts.Remove(script);
        ScriptsUpdated.Invoke();
    }

    private string NewName()
    {
        const string prefix = "Untitled ";
        for (var i = 1; i < 9999; i++)
        {
            var name = $"{prefix}{i}";
            if (Scripts.All(s => s.Trigger.NameJSON.val != name))
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
            script.Trigger.Register();
        }
        ScriptsUpdated.Invoke();
    }
}
