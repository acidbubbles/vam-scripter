using ScripterLang;

public class FloatParamReference : ObjectReference
{
    private readonly StorableProxy _storableRef;
    private readonly string _paramName;

    public FloatParamReference(StorableProxy storableRef, string paramName)
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
        switch (name)
        {
            case "val":
                return GetParam().val;
            case "valNoCallback":
                return GetParam().valNoCallback;
            default:
                return base.GetProperty(name);
        }
    }

    public override void SetProperty(string name, Value value)
    {
        switch (name)
        {
            case "val":
                GetParam().val = value.AsFloat;
                break;
            case "valNoCallback":
                GetParam().valNoCallback = value.AsFloat;
                break;
            default:
                base.GetProperty(name);
                break;
        }
    }
}
