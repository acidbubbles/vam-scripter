using System;
using ScripterLang;

public class DateTimeReference : ObjectReference
{
    private readonly DateTime _value;

    public DateTimeReference(DateTime value)
    {
        _value = value;
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "toString":
                return Func(ToString);
            default:
                return base.GetProperty(name);
        }
    }

    private Value ToString(LexicalContext context, Value[] args)
    {
        return _value.ToString(args.Length == 1 ? args[0].AsString : "s");
    }
}
