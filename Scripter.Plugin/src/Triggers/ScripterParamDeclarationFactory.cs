using System;
using SimpleJSON;

public static class ScripterParamDeclarationFactory
{
    public static ScripterParamDeclarationBase FromJSON(JSONNode json)
    {
        switch (json["Type"].Value)
        {
            case ScripterFloatParamDeclaration.Type:
                return ScripterFloatParamDeclaration.FromJSONImpl(json);
            case ScripterBoolParamDeclaration.Type:
                return ScripterBoolParamDeclaration.FromJSONImpl(json);
            case ScripterStringParamDeclaration.Type:
                return ScripterStringParamDeclaration.FromJSONImpl(json);
            case ScripterActionDeclaration.Type:
                return ScripterActionDeclaration.FromJSONImpl(json);
            default:
                throw new NotSupportedException($"Trigger type {json["Type"].Value} is not supported. Maybe you're running an old version of Scripter?");
        }
    }
}
