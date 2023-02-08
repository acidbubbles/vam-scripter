using ScripterLang;

public class AtomReference : ObjectReference
{
    private readonly Atom _atom;

    public AtomReference(Atom atom)
    {
        _atom = atom;
    }

    public override Value Get(string name)
    {
        switch (name)
        {
            case "getStorable":
                return fn(GetStorable);
            default:
                return base.Get(name);
        }
    }

    private Value GetStorable(RuntimeDomain domain, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetStorable), args, 1);
        var storableName = args[0].AsString;
        var storable = _atom.GetStorableByID(storableName);
        if (storable == null) throw new ScripterPluginException($"Could not find an storable named '{storableName}' in atom '{_atom.storeId}'");
        return new StorableReference(storable);
    }
}
