using ScripterLang;

public class UrlParamReference : ObjectReference
{
    private readonly JSONStorableUrl _param;

    public UrlParamReference(JSONStorableUrl param)
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
