using ScripterLang;
using UnityEngine;

public class MathReference : ObjectReference
{
    private readonly Value _abs = Func((ctx, args) => Mathf.Abs(args[0].AsNumber));
    private readonly Value _acos = Func((ctx, args) => Mathf.Acos(args[0].AsNumber));
    private readonly Value _asin = Func((ctx, args) => Mathf.Asin(args[0].AsNumber));
    private readonly Value _atan = Func((ctx, args) => Mathf.Atan(args[0].AsNumber));
    private readonly Value _atan2 = Func((ctx, args) => Mathf.Atan2(args[0].AsNumber, args[1].AsNumber));
    private readonly Value _ceil = Func((ctx, args) => Mathf.Ceil(args[0].AsNumber));
    private readonly Value _cos = Func((ctx, args) => Mathf.Cos(args[0].AsNumber));
    private readonly Value _exp = Func((ctx, args) => Mathf.Exp(args[0].AsNumber));
    private readonly Value _floor = Func((ctx, args) => Mathf.Floor(args[0].AsNumber));
    private readonly Value _log = Func((ctx, args) => Mathf.Log(args[0].AsNumber));
    private readonly Value _log10 = Func((ctx, args) => Mathf.Log10(args[0].AsNumber));
    private readonly Value _max = Func((ctx, args) => Mathf.Max(args[0].AsNumber));
    private readonly Value _min = Func((ctx, args) => Mathf.Min(args[0].AsNumber));
    private readonly Value _pow = Func((ctx, args) => Mathf.Pow(args[0].AsNumber, args[1].AsNumber));
    private readonly Value _random = Func((ctx, args) => Random.value);
    private readonly Value _round = Func((ctx, args) => Mathf.Round(args[0].AsNumber));
    private readonly Value _sign = Func((ctx, args) => Mathf.Sign(args[0].AsNumber));
    private readonly Value _sin = Func((ctx, args) => Mathf.Sin(args[0].AsNumber));
    private readonly Value _sqrt = Func((ctx, args) => Mathf.Sqrt(args[0].AsNumber));
    private readonly Value _tan = Func((ctx, args) => Mathf.Tan(args[0].AsNumber));

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "abs": return _abs;
            case "acos": return _acos;
            case "asin": return _asin;
            case "atan": return _atan;
            case "atan2": return _atan2;
            case "ceil": return _ceil;
            case "cos": return _cos;
            case "exp": return _exp;
            case "floor": return _floor;
            case "log": return _log;
            case "log10": return _log10;
            case "max": return _max;
            case "min": return _min;
            case "pow": return _pow;
            case "random": return _random;
            case "round": return _round;
            case "sign": return _sign;
            case "sin": return _sin;
            case "sqrt": return _sqrt;
            case "tan": return _tan;
            default: return base.GetProperty(name);
        }
    }

}
