using ScripterLang;

public class StringChooserParamReference : ObjectReference
{
    private readonly JSONStorableStringChooser _param;

    public StringChooserParamReference(JSONStorableStringChooser param)
    {
        _param = param;
    }

    public override Value Get(string name)
    {
#warning choices
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
