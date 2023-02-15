using System.Globalization;
using SimpleJSON;

public class ScripterFloatParam : ScripterParamBase
{
    public const string Type = "FloatParam";

    private readonly JSONStorableFloat _valueJSON;

    public ScripterFloatParam(string name, float startingValue, float minValue, float axValue, bool constrain)
    {
        _valueJSON = new JSONStorableFloat(name, startingValue, minValue, axValue, constrain);
    }

    public static ScripterParamBase FromJSONImpl(JSONNode json)
    {
        var trigger = new ScripterFloatParam(
            json["Name"],
            json["StartingValue"].AsFloat,
            json["MinValue"].AsFloat,
            json["MaxValue"].AsFloat,
            json["Constrain"].AsBool
        );
        trigger._valueJSON.val = json["Val"].AsFloat;
        return trigger;
    }

    public override JSONClass GetJSON()
    {
        var json = new JSONClass
        {
            { "Name", _valueJSON.name },
            { "StartingValue", _valueJSON.defaultVal.ToString(CultureInfo.InvariantCulture) },
            { "MinValue", _valueJSON.min.ToString(CultureInfo.InvariantCulture) },
            { "MaxValue", _valueJSON.max.ToString(CultureInfo.InvariantCulture) },
            { "Constrain", _valueJSON.constrained.ToString() },
            { "Val", _valueJSON.val.ToString(CultureInfo.InvariantCulture) },
        };
        return json;
    }
}
