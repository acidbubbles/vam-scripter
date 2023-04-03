using ScripterLang;
using UnityEngine;

public class TransformReference : ObjectReference
{
    public readonly Transform transform;
    private readonly Value _distance;

    public TransformReference(Transform transform)
    {
        this.transform = transform;
        _distance = Func(Distance);
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "distance":
                return _distance;
            default:
                return base.GetProperty(name);
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
