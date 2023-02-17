using ScripterLang;
using UnityEngine;

public class ControllerReference : ObjectReference
{
    private readonly FreeControllerV3 _controller;

    public ControllerReference(FreeControllerV3 controller)
    {
        _controller = controller;
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "distance":
                return Func(Distance);
            case "rotateTowards":
                return Func(RotateTowards);
            case "moveTowards":
                return Func(MoveTowards);
            default:
                return base.GetProperty(name);
        }
    }

    private Value Distance(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(Distance), args, 1);
        var other = args[0].AsObject as ControllerReference;
        if (ReferenceEquals(other, null)) throw new ScripterRuntimeException($"Expected a ControllerReference as argument to {nameof(Distance)}");
        return Vector3.Distance(_controller.control.position, other._controller.control.position);
    }

    private Value RotateTowards(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(RotateTowards), args, 1);
        var other = args[0].AsObject as ControllerReference;
        if (ReferenceEquals(other, null)) throw new ScripterRuntimeException($"Expected a ControllerReference as argument to {nameof(RotateTowards)}");
        var maxDistanceDelta = args[1].AsNumber;
        _controller.control.position = Vector3.MoveTowards(_controller.control.position, other._controller.control.position, maxDistanceDelta);
        return Value.Void;
    }

    private Value MoveTowards(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(MoveTowards), args, 1);
        var other = args[0].AsObject as ControllerReference;
        if (ReferenceEquals(other, null)) throw new ScripterRuntimeException($"Expected a ControllerReference as argument to {nameof(MoveTowards)}");
        var maxDegreesDelta = args[1].AsNumber;
        _controller.control.rotation = Quaternion.RotateTowards(_controller.control.rotation, other._controller.control.rotation, maxDegreesDelta);
        return Value.Void;
    }
}
