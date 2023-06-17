using System.Collections.Generic;
using ScripterLang;

public class StringChooserParamReference : ObjectReference
{
    private readonly StorableReference _storableRef;
    private readonly string _paramName;

    public StringChooserParamReference(StorableReference storableRef, string paramName)
    {
        _storableRef = storableRef;
        _paramName = paramName;
    }

    private JSONStorableStringChooser GetParam()
    {
        var storable = _storableRef.GetStorable();
        var param = storable.GetStringChooserJSONParam(_paramName);
        if (param == null) throw new ScripterRuntimeException($"Bool param {_paramName} not found in {storable.name} of atom {storable.containingAtom.storeId}");
        return param;
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "val":
                return GetParam().val;
            case "choices":
                var raw = GetParam().choices;
                var values = new List<Value>(raw.Count);
                for (var i = 0; i < raw.Count; i++)
                {
                    values.Add(raw[i]);
                }
                return new ListReference(values);
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
