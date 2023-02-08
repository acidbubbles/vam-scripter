using ScripterLang;
using UnityEngine;

public class TimeReference : ObjectReference
{
    public override Value Get(string name)
    {
        switch (name)
        {
            case "time":
                return fn((d, a) => Time.time);
            case "deltaTime":
                return fn((d, a) => Time.deltaTime);
            default:
                return base.Get(name);
        }
    }
}
