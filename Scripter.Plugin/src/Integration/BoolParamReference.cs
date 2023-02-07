using ScripterLang;

public class BoolParamReference : Reference
{
    private readonly JSONStorableBool _param;

    public BoolParamReference(JSONStorableBool param)
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
        if (name == "val") _param.val = value.AsBool;
        else base.Get(name);
    }
}
