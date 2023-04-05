using System.Runtime.CompilerServices;
using ScripterLang;
using UnityEngine;

public abstract class Vector3Reference : ObjectReference
{
    public abstract Vector3 Vector { get; protected set; }

    private readonly Value _distance;
    private readonly Value _set;

    protected Vector3Reference()
    {
        _distance = Func(Distance);
        _set = Func(Set);
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "x":
                return Vector.x;
            case "y":
                return Vector.y;
            case "z":
                return Vector.z;
            case "distance":
                return _distance;
            case "set":
                return _set;
            default:
                return base.GetProperty(name);
        }
    }

    public override void SetProperty(string name, Value value)
    {
        switch (name)
        {
            case "x":
            {
                var current = Vector;
                current.x = value.AsFloat;
                Vector = current;
                break;
            }
            case "y":
            {
                var current = Vector;
                current.y = value.AsFloat;
                Vector = current;
                break;
            }
            case "z":
            {
                var current = Vector;
                current.z = value.AsFloat;
                Vector = current;
                break;
            }
            default:
                base.SetProperty(name, value);
                break;
        }
    }

    private Value Distance(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(Distance), args, 1);
        var other = args[0].AsObject as Vector3Reference;
        if (ReferenceEquals(other, null)) throw new ScripterRuntimeException($"Expected a ControllerReference as argument to {nameof(Distance)}");
        return Vector3.Distance(Vector, other.Vector);
    }

    private Value Set(LexicalContext context, Value[] args)
    {
        var target = GetVector3Arg(args, nameof(Set));
        Vector = target;
        return Value.Void;
    }

    [MethodImpl(0x0100)]
    private static Vector3 GetVector3Arg(Value[] args, string name)
    {
        Vector3 target;
        if (args.Length == 1)
            target = ((Vector3Reference)args[0].AsObject).Vector;
        else if (args.Length == 3)
            target = new Vector3(args[0].AsFloat, args[1].AsFloat, args[2].AsFloat);
        else
            throw new ScripterRuntimeException($"Method {name} Expected 1 (Vector3) or 3 (float) arguments, received {args.Length}");
        return target;
    }
}
