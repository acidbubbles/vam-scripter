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
        if (name == "val") return _param.val;
        return base.Get(name);
    }

    public override void Set(string name, Value value)
    {
        if (name == "val") _param.val = value.AsString;
        else base.Get(name);
    }
}
