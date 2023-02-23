using System;
using SimpleJSON;

public static class DeclarationFactory
{
    public static ParamDeclarationBase FromJSON(JSONNode json)
    {
        switch (json["Type"].Value)
        {
            case FloatParamDeclaration.Type:
                return FloatParamDeclaration.FromJSONImpl(json);
            case BoolParamDeclaration.Type:
                return BoolParamDeclaration.FromJSONImpl(json);
            case StringParamDeclaration.Type:
                return StringParamDeclaration.FromJSONImpl(json);
            case ActionDeclaration.Type:
                return ActionDeclaration.FromJSONImpl(json);
            default:
                throw new NotSupportedException($"Trigger type {json["Type"].Value} is not supported. Maybe you're running an old version of Scripter?");
        }
    }
}
