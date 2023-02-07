using ScripterLang;

public class SceneReference : Reference
{
    public override Value InvokeMethod(string name, Value[] args)
    {
        switch (name)
        {
            case "getAtom":
                ValidateArgumentsLength(name, args, 1);
                var atomName = args[0].AsString;
                var atom = SuperController.singleton.GetAtomByUid(atomName);
                if (atom == null) throw new ScripterPluginException($"Could not find an atom named '{atomName}'");
                return new AtomReference(atom);
            default:
                return base.InvokeMethod(name, args);
        }
    }
}
