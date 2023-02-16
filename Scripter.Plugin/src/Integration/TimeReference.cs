using ScripterLang;
using UnityEngine;

public class TimeReference : ObjectReference
{
    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "time":
                return Time.time;
            case "deltaTime":
                return Time.deltaTime;
            default:
                return base.GetProperty(name);
        }
    }
}
