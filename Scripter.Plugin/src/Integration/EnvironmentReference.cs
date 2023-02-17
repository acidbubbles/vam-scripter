using ScripterLang;

public class EnvironmentReference : ObjectReference
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
