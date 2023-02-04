using System.Collections.Generic;

public class ScriptsManager
{
    private readonly Dictionary<string, Script> _scripts = new Dictionary<string, Script>();

    public void Add(string name)
    {
        _scripts.Add(name, new Script());
    }

    public Script ByName(string name)
    {
        return _scripts[name];
    }
}
