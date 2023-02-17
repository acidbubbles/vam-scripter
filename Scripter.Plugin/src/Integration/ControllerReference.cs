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
            case "lookAt":
                return Func(LookAt);
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

    private Value LookAt(LexicalContext context, Value[] args)
    {
        if (_controller.isGrabbing) return Value.Void;
        var other = args[0].AsObject as ControllerReference;
        if (ReferenceEquals(other, null)) throw new ScripterRuntimeException($"Expected a ControllerReference as argument to {nameof(LookAt)}");
        var target = other._controller.control.position - _controller.control.position;
        var targetRotation = Quaternion.LookRotation(target, Vector3.up);
        if (args.Length > 1)
        {
            var maxDegreesDelta = args[1].AsNumber;
            targetRotation = Quaternion.RotateTowards(_controller.control.rotation, targetRotation, maxDegreesDelta);
        }
        _controller.control.rotation = targetRotation;
        return Value.Void;
    }

    private Value MoveTowards(LexicalContext context, Value[] args)
    {
        if (_controller.isGrabbing) return Value.Void;
        var other = args[0].AsObject as ControllerReference;
        if (ReferenceEquals(other, null)) throw new ScripterRuntimeException($"Expected a ControllerReference as argument to {nameof(MoveTowards)}");
        var targetPosition = other._controller.control.position;
        if (args.Length > 1)
        {
            var maxDistanceDelta = args[1].AsNumber;
            targetPosition = Vector3.MoveTowards(_controller.control.position, targetPosition, maxDistanceDelta);
        }
        _controller.control.position = targetPosition;
        return Value.Void;
    }
}
