using System.Collections.Generic;
using ScripterLang;
using SimpleJSON;

public class JSON : ObjectReference
{
    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "parse":
                return Func(Parse);
            case "stringify":
                return Func(Stringify);
            default:
                return base.GetProperty(name);
        }
    }

    private Value Parse(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(Parse), args, 1);
        var value = args[0].AsString;
        var jc = JSONNode.Parse(value).AsObject;
        var dict = new Dictionary<string, Value>();
        foreach (var key in jc.Keys)
        {
            var fieldValue = jc[key];
            if(!(fieldValue is JSONData)) throw new ScripterRuntimeException("Deep object serialization not supported");
            dict[key] = Value.CreateString(fieldValue.Value);
        }
        return new MapReference(dict);
    }

    private Value Stringify(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(Parse), args, 1);
        var value = args[0].AsObject as MapReference;
        if (value == null) throw new ScripterRuntimeException("Only javascript objects can be serialized");
        var jc = new JSONClass();
        foreach (var field in value.GetPropertyNames())
        {
            var fieldValue = value.GetProperty(field);
            if (fieldValue.IsObject) throw new ScripterRuntimeException("Deep object serialization not supported");
            jc[field] = fieldValue.ToString();
        }
        return jc.ToString();
    }
}
