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
    private readonly Value _clamp = Func((ctx, args) => Mathf.Clamp(args[0].AsNumber, args[1].AsNumber, args[2].AsNumber));
    private readonly Value _clamp01 = Func((ctx, args) => Mathf.Clamp01(args[0].AsNumber));
    private readonly Value _cos = Func((ctx, args) => Mathf.Cos(args[0].AsNumber));
    private readonly Value _exp = Func((ctx, args) => Mathf.Exp(args[0].AsNumber));
    private readonly Value _floor = Func((ctx, args) => Mathf.Floor(args[0].AsNumber));
    private readonly Value _inverseLerp = Func((ctx, args) => Mathf.InverseLerp(args[0].AsNumber, args[1].AsNumber, args[2].AsNumber));
    private readonly Value _lerp = Func((ctx, args) => Mathf.Lerp(args[0].AsNumber, args[1].AsNumber, args[2].AsNumber));
    private readonly Value _lerpAngle = Func((ctx, args) => Mathf.LerpAngle(args[0].AsNumber, args[1].AsNumber, args[2].AsNumber));
    private readonly Value _lerpUnclamped = Func((ctx, args) => Mathf.LerpUnclamped(args[0].AsNumber, args[1].AsNumber, args[2].AsNumber));
    private readonly Value _log = Func((ctx, args) => Mathf.Log(args[0].AsNumber));
    private readonly Value _log10 = Func((ctx, args) => Mathf.Log10(args[0].AsNumber));
    private readonly Value _max = Func(Max);
    private readonly Value _min = Func(Min);
    private readonly Value _pingPong = Func((ctx, args) => Mathf.PingPong(args[0].AsNumber, args[1].AsNumber));
    private readonly Value _pow = Func((ctx, args) => Mathf.Pow(args[0].AsNumber, args[1].AsNumber));
    private readonly Value _random = Func((ctx, args) => Random.value);
    private readonly Value _round = Func((ctx, args) => Mathf.Round(args[0].AsNumber));
    private readonly Value _sign = Func((ctx, args) => Mathf.Sign(args[0].AsNumber));
    private readonly Value _sin = Func((ctx, args) => Mathf.Sin(args[0].AsNumber));
    private readonly Value _smoothStep = Func((ctx, args) => Mathf.SmoothStep(args[0].AsNumber, args[1].AsNumber, args[2].AsNumber));
    private readonly Value _sqrt = Func((ctx, args) => Mathf.Sqrt(args[0].AsNumber));
    private readonly Value _tan = Func((ctx, args) => Mathf.Tan(args[0].AsNumber));

    private static Value Max(LexicalContext context, Value[] args)
    {
        var first = args[0];
        if (first.IsObject)
        {
            var list = (ListReference)first.AsObject;
            var floats = new float[list.values.Count];
            for(var i = 0; i < floats.Length; i++)
                floats[i] = list.values[i].AsNumber;
            return Mathf.Max(floats);
        }
        else
        {
            var floats = new float[args.Length];
            for(var i = 0; i < floats.Length; i++)
                floats[i] = args[i].AsNumber;
            return Mathf.Max(floats);
        }
    }

    private static Value Min(LexicalContext context, Value[] args)
    {
        var first = args[0];
        if (first.IsObject)
        {
            var list = (ListReference)first.AsObject;
            var floats = new float[list.values.Count];
            for(var i = 0; i < floats.Length; i++)
                floats[i] = list.values[i].AsNumber;
            return Mathf.Min(floats);
        }
        else
        {
            var floats = new float[args.Length];
            for(var i = 0; i < floats.Length; i++)
                floats[i] = args[i].AsNumber;
            return Mathf.Min(floats);
        }
    }

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
            case "clamp" : return _clamp;
            case "clamp01" : return _clamp01;
            case "cos": return _cos;
            case "exp": return _exp;
            case "floor": return _floor;
            case "inverseLerp": return _inverseLerp;
            case "lerp": return _lerp;
            case "lerpAngle": return _lerpAngle;
            case "lerpUnclamped": return _lerpUnclamped;
            case "log": return _log;
            case "log10": return _log10;
            case "max": return _max;
            case "min": return _min;
            case "pingPong": return _pingPong;
            case "pow": return _pow;
            case "random": return _random;
            case "round": return _round;
            case "sign": return _sign;
            case "sin": return _sin;
            case "smoothStep": return _smoothStep;
            case "sqrt": return _sqrt;
            case "tan": return _tan;
            default: return base.GetProperty(name);
        }
    }

}
