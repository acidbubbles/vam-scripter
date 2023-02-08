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
        if (name == "val") return _param.val;
        return base.Get(name);
    }

    public override void Set(string name, Value value)
    {
        if (name == "val") _param.val = value.AsString;
        else base.Get(name);
    }
}
