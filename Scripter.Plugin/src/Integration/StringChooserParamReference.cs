using ScripterLang;

public class StringChooserParamReference : ObjectReference
{
    private readonly JSONStorableStringChooser _param;

    public StringChooserParamReference(JSONStorableStringChooser param)
    {
        _param = param;
    }

    public override Value GetProperty(string name)
    {
#warning choices
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
