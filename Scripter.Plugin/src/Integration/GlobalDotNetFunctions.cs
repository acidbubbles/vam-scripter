using System;
using ScripterLang;

public static class GlobalDotNetFunctions
{
    public static void Register(GlobalLexicalContext lexicalContext)
    {
        lexicalContext.Functions.Add("getDateTime", GetDateTime);
    }

    private static Value GetDateTime(RuntimeDomain domain, Value[] args)
    {
        var now = DateTime.Now;
        if (args.Length == 0)
            return now.ToString("s");
        else
            return now.ToString(args[0].AsString);
    }
}
