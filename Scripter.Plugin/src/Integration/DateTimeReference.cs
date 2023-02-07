using System;
using ScripterLang;

public class DateTimeReference : Reference
{
    private readonly DateTime _value;

    public DateTimeReference(DateTime value)
    {
        _value = value;
    }

    public override Value InvokeMethod(string name, Value[] args)
    {
        ValidateArgumentsLength(name, args, 1);
        switch (name)
        {
            case "toString":
                return _value.ToString(args.Length == 1 ? args[0].AsString : "s");
            default:
                return base.InvokeMethod(name, args);
        }
    }
}
