using ScripterLang;
using UnityEngine;

public static class GlobalUnityFunctions
{
    public static void Register(GlobalLexicalContext lexicalContext)
    {
        lexicalContext.Functions.Add("getTime", (d, a) => Time.time);
        lexicalContext.Functions.Add("getDeltaTime", (d, a) => Time.deltaTime);
        lexicalContext.Functions.Add("getRandom", GetRandom);
    }

    private static Value GetRandom(RuntimeDomain domain, Value[] args)
    {
        if (args.Length == 0)
            return Random.value;
        Reference.ValidateArgumentsLength(nameof(GetRandom), args, 2);
        return Random.Range(args[0].AsFloat, args[1].AsFloat);
    }
}
