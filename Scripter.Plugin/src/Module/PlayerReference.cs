using ScripterLang;

public class PlayerReference : ObjectReference
{
    private readonly TransformReference _lHand = new TransformReference(SuperController.singleton.leftHand);
    private readonly TransformReference _rHand = new TransformReference(SuperController.singleton.rightHand);
    private readonly TransformReference _head = new TransformReference(SuperController.singleton.centerCameraTarget.transform);

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "isVR":
                return !SuperController.singleton.IsMonitorRigActive;
            case "lHand":
                return _lHand;
            case "rHand":
                return _rHand;
            case "head":
                return _head;
            default:
                return base.GetProperty(name);
        }
    }
}
