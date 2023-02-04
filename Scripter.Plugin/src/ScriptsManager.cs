using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class ScriptsManager
{
    public readonly UnityEvent ScriptsUpdated = new UnityEvent();
    public readonly List<Script> Scripts = new List<Script>();

    public void Add()
    {
        var name = NewName();
        Scripts.Add(new Script(name));
        ScriptsUpdated.Invoke();
    }

    public void Delete(Script script)
    {
        Scripts.Remove(script);
        ScriptsUpdated.Invoke();
    }

    private string NewName()
    {
        const string prefix = "Untitled ";
        for (var i = 1; i < 9999; i++)
        {
            var name = $"{prefix}{i}";
            if (Scripts.All(s => s.NameJSON.val != name))
                return name;
        }
        throw new InvalidOperationException("You're creating way too many scripts!");
    }
}
