using System.Linq;
using ScripterLang;

public class AtomReference : ObjectReference
{
    private readonly Atom _atom;

    public AtomReference(Atom atom)
    {
        _atom = atom;
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "getStorable":
                return Func(GetStorable);
            case "getController":
                return Func(GetController);
            default:
                return base.GetProperty(name);
        }
    }

    private Value GetStorable(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetStorable), args, 1);
        var storableName = args[0].AsString;
        var storable = _atom.GetStorableByID(storableName);
        if (storable == null) throw new ScripterPluginException($"Could not find an storable named '{storableName}' in atom '{_atom.storeId}'");
        return new StorableReference(storable);
    }

    private Value GetController(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetStorable), args, 1);
        var controllerName = args[0].AsString;
        var controller = _atom.freeControllers.FirstOrDefault(fc => fc.name == controllerName);
        if (controller == null) throw new ScripterPluginException($"Could not find an storable named '{controllerName}' in atom '{_atom.storeId}'");
        return new ControllerReference(controller);
    }
}
