using ScripterLang;

public class StorableReference : ObjectReference
{
    private readonly JSONStorable _storable;

    public StorableReference(JSONStorable storable)
    {
        _storable = storable;
    }

    public override Value Get(string name)
    {
        switch (name)
        {
            case "trigger":
                return fn(Trigger);
            case "getFloat":
                return fn(GetFloat);
            case "getString":
                return fn(GetString);
            case "getStringChooser":
                return fn(GetStringChooser);
            case "getBool":
                return fn(GetBool);
            default:
                return base.Get(name);
        }
    }

    public Value Trigger(RuntimeDomain domain, Value[] args)
    {
        ValidateArgumentsLength(nameof(Trigger), args, 1);
        var paramName = args[0].AsString;
        _storable.CallAction(paramName);
        return Value.Void;
    }

    public Value GetFloat(RuntimeDomain domain, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetFloat), args, 1);
        var paramName = args[0].AsString;
        var param = _storable.GetFloatJSONParam(paramName);
        if (param == null)
            throw new ScripterPluginException(
                $"Could not find a float param named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
        return new FloatParamReference(param);
    }

    public Value GetString(RuntimeDomain domain, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetString), args, 1);
        var paramName = args[0].AsString;
        var param = _storable.GetStringJSONParam(paramName);
        if (param == null)
            throw new ScripterPluginException(
                $"Could not find a float param named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
        return new StringParamReference(param);
    }

    public Value GetStringChooser(RuntimeDomain domain, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetStringChooser), args, 1);
        var paramName = args[0].AsString;
        var param = _storable.GetStringChooserJSONParam(paramName);
        if (param == null)
            throw new ScripterPluginException(
                $"Could not find a float param named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
        return new StringChooserParamReference(param);
    }

    public Value GetBool(RuntimeDomain domain, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetBool), args, 1);
        var paramName = args[0].AsString;
        var param = _storable.GetBoolJSONParam(paramName);
        if (param == null)
            throw new ScripterPluginException(
                $"Could not find a float param named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
        return new BoolParamReference(param);
    }
}
