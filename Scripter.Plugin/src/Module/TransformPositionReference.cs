using UnityEngine;

public class TransformPositionReference : Vector3Reference
{
    private readonly Transform _transform;

    public override Vector3 Vector
    {
        get { return _transform.position; }
        protected set { _transform.position = value; }
    }

    public TransformPositionReference(Transform transform)
    {
        _transform = transform;
    }
}

public class TransformLocalPositionReference : Vector3Reference
{
    private readonly Transform _transform;

    public override Vector3 Vector
    {
        get { return _transform.localPosition; }
        protected set { _transform.localPosition = value; }
    }

    public TransformLocalPositionReference(Transform transform)
    {
        _transform = transform;
    }
}

public class TransformEulerAnglesReference : Vector3Reference
{
    private readonly Transform _transform;

    public override Vector3 Vector
    {
        get { return _transform.eulerAngles; }
        protected set { _transform.eulerAngles = value; }
    }

    public TransformEulerAnglesReference(Transform transform)
    {
        _transform = transform;
    }
}

public class TransformLocalEulerAnglesReference : Vector3Reference
{
    private readonly Transform _transform;

    public override Vector3 Vector
    {
        get { return _transform.localEulerAngles; }
        protected set { _transform.localEulerAngles = value; }
    }

    public TransformLocalEulerAnglesReference(Transform transform)
    {
        _transform = transform;
    }
}
