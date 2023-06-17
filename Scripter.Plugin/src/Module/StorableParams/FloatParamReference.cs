using ScripterLang;

public class FloatParamReference : ObjectReference
{
    private readonly StorableReference _storableRef;
    private readonly string _paramName;

    public FloatParamReference(StorableReference storableRef, string paramName)
    {
        _storableRef = storableRef;
        _paramName = paramName;
    }

    private JSONStorableFloat GetParam()
    {
        var storable = _storableRef.GetStorable();
        var param = storable.GetFloatJSONParam(_paramName);
        if (param == null) throw new ScripterRuntimeException($"Bool param {_paramName} not found in {storable.name} of atom {storable.containingAtom.storeId}");
        return param;
    }

    public override Value GetProperty(string name)
    {
        if (name == "val") return GetParam().val;
        return base.GetProperty(name);
    }

    public override void SetProperty(string name, Value value)
    {
        if (name == "val") GetParam().val = value.AsNumber;
        else base.GetProperty(name);
    }
}
