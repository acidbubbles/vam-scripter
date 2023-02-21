using System.Collections.Generic;
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
            case "name":
                return _atom.name;
            case "type":
                return _atom.type;
            case "on":
                return _atom.on;
            case "getStorableIds":
                return Func(GetStorableIds);
            case "getStorable":
                return Func(GetStorable);
            case "getController":
                return Func(GetController);
            default:
                return base.GetProperty(name);
        }
    }

    public override void SetProperty(string name, Value value)
    {
        switch (name)
        {
            case "on":
                _atom.SetBoolParamValue("on", value.AsBool);
                break;
            default:
                base.SetProperty(name, value);
                break;
        }
    }

    private Value GetStorableIds(LexicalContext context, Value[] args)
    {
        var raw = _atom.GetStorableIDs();
        var values = new List<Value>(raw.Count);
        for (var i = 0; i < raw.Count; i++)
        {
            values[i] = raw[i];
        }
        return new ListReference(values);
    }

    private Value GetStorable(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetStorable), args, 1);
        var storableName = args[0].AsString;
        var storable = _atom.GetStorableByID(storableName);
        if (storable == null) throw new ScripterPluginException($"Could not find an storable named '{storableName}' in atom '{_atom.name}'");
        return new StorableReference(storable);
    }

    private Value GetController(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetStorable), args, 1);
        var controllerName = args[0].AsString;
        var controller = _atom.freeControllers.FirstOrDefault(fc => fc.name == controllerName);
        if (controller == null) throw new ScripterPluginException($"Could not find an storable named '{controllerName}' in atom '{_atom.name}'");
        return new ControllerReference(controller);
    }
}
