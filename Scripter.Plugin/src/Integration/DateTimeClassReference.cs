using System;
using ScripterLang;

public class DateTimeClassReference : ObjectReference
{
    public override Value Get(string name)
    {
        switch (name)
        {
            case "now":
                return new DateTimeReference(DateTime.Now);
            default:
                return base.Get(name);
        }
    }
}
