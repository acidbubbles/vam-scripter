using ScripterLang;

public class AtomReference : ObjectReference
{
    private readonly Atom _atom;

    public AtomReference(Atom atom)
    {
        _atom = atom;
    }

    public override Value InvokeMethod(string name, Value[] args)
    {
        switch (name)
        {
            case "getStorable":
                ValidateArgumentsLength(name, args, 1);
                var storableName = args[0].AsString;
                var storable = _atom.GetStorableByID(storableName);
                if (storable == null) throw new ScripterPluginException($"Could not find an storable named '{storableName}' in atom '{_atom.storeId}'");
                return new StorableReference(storable);
            default:
                return base.InvokeMethod(name, args);
        }
    }
}
