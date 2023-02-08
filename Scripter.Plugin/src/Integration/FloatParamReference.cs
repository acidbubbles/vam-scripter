using ScripterLang;

public class FloatParamReference : ObjectReference
{
    private readonly JSONStorableFloat _param;

    public FloatParamReference(JSONStorableFloat param)
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
        if (name == "val") _param.val = value.AsNumber;
        else base.Get(name);
    }
}
