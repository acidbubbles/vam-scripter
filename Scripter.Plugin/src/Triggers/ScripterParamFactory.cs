using System;
using SimpleJSON;

public static class ScripterParamFactory
{
    public static ScripterParamBase FromJSON(JSONNode json)
    {
        switch (json["Type"].Value)
        {
            case ScripterFloatParam.Type:
                return ScripterFloatParam.FromJSONImpl(json);
            default:
                throw new NotSupportedException($"Trigger type {json["Type"].Value} is not supported. Maybe you're running an old version of Scripter?");
        }
    }
}
