using ScripterLang;

public class PlayerReference : ObjectReference
{
    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "isVR":
                return !SuperController.singleton.IsMonitorRigActive;
            default:
                return base.GetProperty(name);
        }
    }
}
