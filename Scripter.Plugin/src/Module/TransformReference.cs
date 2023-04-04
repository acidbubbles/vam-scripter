using ScripterLang;
using UnityEngine;

public class TransformReference : ObjectReference
{
    public readonly Transform transform;
    private readonly Value _distance;
    private readonly Value _position;
    private readonly Value _localPosition;
    private readonly Value _eulerAngles;
    private readonly Value _localEulerAngles;

    public TransformReference(Transform transform)
    {
        this.transform = transform;
        _position = Value.CreateObject(new TransformPositionReference(transform));
        _localPosition = Value.CreateObject(new TransformLocalPositionReference(transform));
        _eulerAngles = Value.CreateObject(new TransformEulerAnglesReference(transform));
        _localEulerAngles = Value.CreateObject(new TransformLocalEulerAnglesReference(transform));
        _distance = Func(Distance);
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "distance":
                return _distance;
            case "position":
                return _position;
            case "localPosition":
                return _localPosition;
            case "eulerAngles":
                return _eulerAngles;
            case "localEulerAngles":
                return _localEulerAngles;
            default:
                return base.GetProperty(name);
        }
    }

    public override void SetProperty(string name, Value value)
    {        switch (name)
        {
            case "position":
                transform.position = ((Vector3Reference)value.AsObject).Vector;
                break;
            case "localPosition":
                transform.localPosition = ((Vector3Reference)value.AsObject).Vector;
                break;
            case "eulerAngles":
                transform.eulerAngles = ((Vector3Reference)value.AsObject).Vector;
                break;
            case "localEulerAngles":
                transform.localEulerAngles = ((Vector3Reference)value.AsObject).Vector;
                break;
            default:
                base.SetProperty(name, value);
                break;
        }
    }

    private Value Distance(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(Distance), args, 1);
        var other = args[0].AsObject as TransformReference;
        if (ReferenceEquals(other, null)) throw new ScripterRuntimeException($"Expected a ControllerReference as argument to {nameof(Distance)}");
        return Vector3.Distance(transform.position, other.transform.position);
    }
}
