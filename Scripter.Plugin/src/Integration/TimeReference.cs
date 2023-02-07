using ScripterLang;
using UnityEngine;

public class TimeReference : Reference
{
    public override Value InvokeMethod(string name, Value[] args)
    {
        ValidateArgumentsLength(name, args, 1);
        switch (name)
        {
            case "time":
                return Time.time;
            case "deltaTime":
                return Time.deltaTime;
            default:
                return base.InvokeMethod(name, args);
        }
    }
}
