using ScripterLang;

public class FloatParamReference : ObjectReference
{
    private readonly JSONStorableFloat _param;

    public FloatParamReference(JSONStorableFloat param)
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
        if (name == "val") _param.val = value.AsNumber;
        else base.GetProperty(name);
    }
}
