using System.Collections.Generic;
using ScripterLang;

public class StringChooserParamReference : ObjectReference
{
    private readonly JSONStorableStringChooser _param;

    public StringChooserParamReference(JSONStorableStringChooser param)
    {
        _param = param;
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "val":
                return _param.val;
            case "choices":
                var raw = _param.choices;
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
                _param.val = value.AsString;
                break;
            default:
                base.GetProperty(name);
                break;
        }
    }
}
