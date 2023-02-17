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
            default:
                return base.GetProperty(name);
        }
    }

    private Value Distance(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(Distance), args, 1);
        var other = args[0].AsObject as ControllerReference;
        if (other == null) throw new ScripterRuntimeException($"Expected a ControllerReference as argument to {nameof(Distance)}");
        return Vector3.Distance(_controller.control.position, other._controller.control.position);
    }
}
