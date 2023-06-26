using ScripterLang;
using UnityEngine;
using UnityEngine.UI.Extensions.ColorPicker;

public class ColorParamReference : ObjectReference
{
    private readonly StorableReference _storableRef;
    private readonly string _paramName;

    public ColorParamReference(StorableReference storableRef, string paramName)
    {
        _storableRef = storableRef;
        _paramName = paramName;
    }

    private JSONStorableColor GetParam()
    {
        var storable = _storableRef.GetStorable();
        var param = storable.GetColorJSONParam(_paramName);
        if (param == null) throw new ScripterRuntimeException($"Bool param {_paramName} not found in {storable.name} of atom {storable.containingAtom.storeId}");
        return param;
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "val":
                return HsvToHtml(GetParam().val);
            case "valNoCallback":
                return HsvToHtml(GetParam().valNoCallback);
            default:
                return base.GetProperty(name);
        }
    }

    public override void SetProperty(string name, Value value)
    {
        switch (name)
        {
            case "val":
                GetParam().val = HtmlToHsv(value);
                break;
            case "valNoCallback":
                GetParam().valNoCallback = HtmlToHsv(value);
                break;
            default:
                base.GetProperty(name);
                break;
        }
    }

    private static HSVColor HtmlToHsv(Value value)
    {
        Color color;
        if (!ColorUtility.TryParseHtmlString(value.AsString, out color))
            throw new ScripterRuntimeException("Invalid color string");
        var hsv = HSVUtil.ConvertRgbToHsv(color);
        return new HSVColor { H = hsv.NormalizedH, S = hsv.NormalizedS, V = hsv.NormalizedV };
    }

    private static Value HsvToHtml(HSVColor hsv)
    {
        var color = HSVUtil.ConvertHsvToRgb(hsv.H, hsv.S, hsv.V, 1f);
        return ColorUtility.ToHtmlStringRGB(color);
    }
}
