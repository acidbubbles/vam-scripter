using ScripterLang;

public class StringParamReference : ObjectReference
{
    private readonly StorableReference _storableRef;
    private readonly string _paramName;

    public StringParamReference(StorableReference storableRef, string paramName)
    {
        _storableRef = storableRef;
        _paramName = paramName;
    }

    private JSONStorableString GetParam()
    {
        var storable = _storableRef.GetStorable();
        var param = storable.GetStringJSONParam(_paramName);
        if (param == null) throw new ScripterRuntimeException($"Bool param {_paramName} not found in {storable.name} of atom {storable.containingAtom.storeId}");
        return param;
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "val":
                return GetParam().val;
            default:
                return base.GetProperty(name);
        }
    }

    public override void SetProperty(string name, Value value)
    {
        switch (name)
        {
            case "val":
                GetParam().val = value.AsString;
                break;
            default:
                base.GetProperty(name);
                break;
        }
    }
}
