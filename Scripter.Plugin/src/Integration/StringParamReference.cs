using ScripterLang;

public class StringParamReference : ObjectReference
{
    private readonly JSONStorableString _param;

    public StringParamReference(JSONStorableString param)
    {
        _param = param;
    }

    public override Value Get(string name)
    {
        switch (name)
        {
            case "val":
                return _param.val;
            default:
                return base.Get(name);
        }
    }

    public override void Set(string name, Value value)
    {
        switch (name)
        {
            case "val":
                _param.val = value.AsString;
                break;
            default:
                base.Get(name);
                break;
        }
    }
}
