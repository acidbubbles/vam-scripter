using ScripterLang;
using UnityEngine;

public class TransformReference : ObjectReference
{
    public readonly Transform transform;

    public TransformReference(Transform transform)
    {
        this.transform = transform;
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "distance":
                return Func(Distance);
            default:
                return base.GetProperty(name);
        }
    }

    private Value Distance(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(Distance), args, 1);
        var other = args[0].AsObject as ControllerReference;
        if (ReferenceEquals(other, null)) throw new ScripterRuntimeException($"Expected a ControllerReference as argument to {nameof(Distance)}");
        return Vector3.Distance(transform.position, other.transform.position);
    }
}
