using ScripterLang;
using UnityEngine;

public class RandomReference : Reference
{
    public override Value Get(string name)
    {
        switch (name)
        {
            case "value":
                return Random.value;
            default:
                return base.Get(name);
        }
    }

    public override Value InvokeMethod(string name, Value[] args)
    {
        switch (name)
        {
            case "range":
                ValidateArgumentsLength(name, args, 2);
                if(args[0].IsInt && args[1].IsInt)
                    return Random.Range(args[0].RawInt, args[1].RawInt);
                else
                    return Random.Range(args[0].AsNumber, args[1].AsNumber);
            default:
                return base.InvokeMethod(name, args);
        }
    }
}

public static class GlobalUnityFunctions
{
    public static void Register(GlobalLexicalContext lexicalContext)
    {
        lexicalContext.Globals.Add("Time", new TimeReference());
        lexicalContext.Globals.Add("Random", new TimeReference());
    }
}
