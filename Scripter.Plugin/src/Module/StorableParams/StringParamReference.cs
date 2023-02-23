using ScripterLang;

public class StringParamReference : ObjectReference
{
    private readonly JSONStorableString _param;

    public StringParamReference(JSONStorableString param)
    {
        _param = param;
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "val":
                return _param.val;
            default:
                return base.GetProperty(name);
        }
    }

    public override void SetProperty(string name, Value value)
    {
        switch (name)
        {
            case "val":
                _param.val = value.AsString;
                break;
            default:
                base.GetProperty(name);
                break;
        }
    }
}
