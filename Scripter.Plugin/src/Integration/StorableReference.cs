using ScripterLang;

public class StorableReference : Reference
{
    private readonly JSONStorable _storable;

    public StorableReference(JSONStorable storable)
    {
        _storable = storable;
    }

    public override Value InvokeMethod(string name, Value[] args)
    {
        switch (name)
        {
            case "trigger":
            {
                ValidateArgumentsLength(name, args, 1);
                var paramName = args[0].AsString;
                _storable.CallAction(paramName);
                return Value.Void;
            }
            case "getFloat":
            {
                ValidateArgumentsLength(name, args, 1);
                var paramName = args[0].AsString;
                var param = _storable.GetFloatJSONParam(paramName);
                if (param == null)
                    throw new ScripterPluginException(
                        $"Could not find a float param named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
                return new FloatParamReference(param);
            }
            case "getString":
            {
                ValidateArgumentsLength(name, args, 1);
                var paramName = args[0].AsString;
                var param = _storable.GetStringJSONParam(paramName);
                if (param == null)
                    throw new ScripterPluginException(
                        $"Could not find a float param named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
                return new StringParamReference(param);
            }
            case "getStringChooser":
            {
                ValidateArgumentsLength(name, args, 1);
                var paramName = args[0].AsString;
                var param = _storable.GetStringChooserJSONParam(paramName);
                if (param == null)
                    throw new ScripterPluginException(
                        $"Could not find a float param named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
                return new StringChooserParamReference(param);
            }
            case "getBool":
            {
                ValidateArgumentsLength(name, args, 1);
                var paramName = args[0].AsString;
                var param = _storable.GetBoolJSONParam(paramName);
                if (param == null)
                    throw new ScripterPluginException(
                        $"Could not find a float param named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
                return new BoolParamReference(param);
            }
            default:
            {
                return base.InvokeMethod(name, args);
            }
        }
    }
}
