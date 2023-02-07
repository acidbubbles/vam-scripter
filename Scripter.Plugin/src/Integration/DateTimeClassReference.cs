using System;
using ScripterLang;

public class DateTimeClassReference : Reference
{
    public override Value InvokeMethod(string name, Value[] args)
    {
        ValidateArgumentsLength(name, args, 1);
        switch (name)
        {
            case "Now":
                return new DateTimeReference(DateTime.Now);
            default:
                return base.InvokeMethod(name, args);
        }
    }
}
