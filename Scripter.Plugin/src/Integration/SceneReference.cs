using ScripterLang;

public class SceneReference : ObjectReference
{
    public override Value Get(string name)
    {
        switch (name)
        {
            case "getAtom":
                return fn(GetAtom);
            default:
                return base.Get(name);
        }
    }

    private static Value GetAtom(RuntimeDomain domain, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetAtom), args, 1);
        var atomName = args[0].AsString;
        var atom = SuperController.singleton.GetAtomByUid(atomName);
        if (atom == null) throw new ScripterPluginException($"Could not find an atom named '{atomName}'");
        return new AtomReference(atom);
    }
}
