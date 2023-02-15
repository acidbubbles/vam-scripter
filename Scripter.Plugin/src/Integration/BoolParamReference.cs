using ScripterLang;

public class BoolParamReference : ObjectReference
{
    private readonly JSONStorableBool _param;

    public BoolParamReference(JSONStorableBool param)
    {
        _param = param;
    }

    public override Value GetProperty(string name)
    {
        if (name == "val") return _param.val;
        return base.GetProperty(name);
    }

    public override void SetProperty(string name, Value value)
    {
        if (name == "val") _param.val = value.AsBool;
        else base.GetProperty(name);
    }
}
