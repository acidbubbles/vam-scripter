using ScripterLang;
using UnityEngine;

public class RandomReference : ObjectReference
{
    private static readonly Value _range = Func(Range);

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "value":
                return Random.value;
            case "range":
                return _range;
            default:
                return base.GetProperty(name);
        }
    }

    private static Value Range(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(Range), args, 2);
        if (args[0].IsInt && args[1].IsInt)
            return Random.Range(args[0].RawInt, args[1].RawInt);
        return Random.Range(args[0].AsNumber, args[1].AsNumber);
    }
}

