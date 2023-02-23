using ScripterLang;
using UnityEngine;

public class ControllerReference : TransformReference
{
    private readonly FreeControllerV3 _controller;

    public ControllerReference(FreeControllerV3 controller)
        : base(controller.control)
    {
        _controller = controller;
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "lookAt":
                return Func(LookAt);
            case "moveTowards":
                return Func(MoveTowards);
            default:
                return base.GetProperty(name);
        }
    }

    private Value LookAt(LexicalContext context, Value[] args)
    {
        if (_controller.isGrabbing) return Value.Void;
        var other = args[0].AsObject as TransformReference;
        if (ReferenceEquals(other, null)) throw new ScripterRuntimeException($"Expected a ControllerReference as argument to {nameof(LookAt)}");
        var target = other.transform.position - _controller.control.position;
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
        var other = args[0].AsObject as TransformReference;
        if (ReferenceEquals(other, null)) throw new ScripterRuntimeException($"Expected a ControllerReference as argument to {nameof(MoveTowards)}");
        var targetPosition = other.transform.position;
        if (args.Length > 1)
        {
            var maxDistanceDelta = args[1].AsNumber;
            targetPosition = Vector3.MoveTowards(_controller.control.position, targetPosition, maxDistanceDelta);
        }
        _controller.control.position = targetPosition;
        return Value.Void;
    }
}
