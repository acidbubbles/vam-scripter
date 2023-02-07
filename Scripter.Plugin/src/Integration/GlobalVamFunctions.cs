using System;
using ScripterLang;

public static class GlobalVamFunctions
{
    public static void Register(GlobalLexicalContext lexicalContext)
    {
        lexicalContext.Functions.Add("logMessage", LogMessage);
        lexicalContext.Functions.Add("logError", LogError);
        lexicalContext.Functions.Add("getFloatParamValue", GetFloatParamValue);
        lexicalContext.Functions.Add("setFloatParamValue", SetFloatParamValue);
        lexicalContext.Functions.Add("getBoolParamValue", GetBoolParamValue);
        lexicalContext.Functions.Add("setBoolParamValue", SetBoolParamValue);
        lexicalContext.Functions.Add("getStringParamValue", GetStringParamValue);
        lexicalContext.Functions.Add("setStringParamValue", SetStringParamValue);
        lexicalContext.Functions.Add("getStringChooserParamValue", GetStringChooserParamValue);
        lexicalContext.Functions.Add("setStringChooserParamValue", SetStringChooserParamValue);
        lexicalContext.Functions.Add("invokeTrigger", InvokeTrigger);
        lexicalContext.Functions.Add("invokeKeybinding", InvokeKeybinding);
    }

    private static Value LogMessage(RuntimeDomain domain, Value[] args)
    {
        Reference.ValidateArgumentsLength(nameof(LogMessage), args, 1);
        SuperController.LogMessage("Scripter: " + args[0].AsString);
        return Value.Void;
    }

    private static Value LogError(RuntimeDomain domain, Value[] args)
    {
        Reference.ValidateArgumentsLength(nameof(LogError), args, 1);
        SuperController.LogError(args[0].ToString());
        return Value.Void;
    }

    private static Value GetFloatParamValue(RuntimeDomain domain, Value[] args)
    {
        Reference.ValidateArgumentsLength(nameof(GetFloatParamValue), args, 3);
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].AsString;
        var param = storable.GetFloatJSONParam(paramName);
        if(param == null) throw new ScripterRuntimeException($"FloatParam {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        return Value.CreateFloat(param.val);
    }

    private static Value SetFloatParamValue(RuntimeDomain domain, Value[] args)
    {
        Reference.ValidateArgumentsLength(nameof(SetFloatParamValue), args, 4);
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].AsString;
        var param = storable.GetFloatJSONParam(paramName);
        if(param == null) throw new ScripterRuntimeException($"FloatParam {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        param.val = args[3].AsFloat;
        return args[3];
    }

    private static Value GetBoolParamValue(RuntimeDomain domain, Value[] args)
    {
        Reference.ValidateArgumentsLength(nameof(GetBoolParamValue), args, 3);
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].AsString;
        var param = storable.GetBoolJSONParam(paramName);
        if(param == null) throw new ScripterRuntimeException($"BoolParam {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        return Value.CreateBoolean(param.val);
    }

    private static Value SetBoolParamValue(RuntimeDomain domain, Value[] args)
    {
        Reference.ValidateArgumentsLength(nameof(SetBoolParamValue), args, 4);
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].AsString;
        var param = storable.GetBoolJSONParam(paramName);
        if(param == null) throw new ScripterRuntimeException($"BoolParam {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        param.val = args[3].AsBool;
        return args[3];
    }

    private static Value GetStringParamValue(RuntimeDomain domain, Value[] args)
    {
        Reference.ValidateArgumentsLength(nameof(GetStringParamValue), args, 3);
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].AsString;
        var param = storable.GetStringJSONParam(paramName);
        if(param == null) throw new ScripterRuntimeException($"StringParam {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        return Value.CreateString(param.val);
    }

    private static Value SetStringParamValue(RuntimeDomain domain, Value[] args)
    {
        Reference.ValidateArgumentsLength(nameof(SetStringParamValue), args, 4);
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].AsString;
        var param = storable.GetStringJSONParam(paramName);
        if(param == null) throw new ScripterRuntimeException($"StringParam {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        param.val = args[3].ToString();
        // return args[3];
        return Value.Void;
    }

    private static Value GetStringChooserParamValue(RuntimeDomain domain, Value[] args)
    {
        Reference.ValidateArgumentsLength(nameof(GetStringChooserParamValue), args, 3);
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].AsString;
        var param = storable.GetStringChooserJSONParam(paramName);
        if(param == null) throw new ScripterRuntimeException($"StringChooserParam {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        return Value.CreateString(param.val);
    }

    private static Value SetStringChooserParamValue(RuntimeDomain domain, Value[] args)
    {
        Reference.ValidateArgumentsLength(nameof(SetStringChooserParamValue), args, 4);
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].AsString;
        var param = storable.GetStringChooserJSONParam(paramName);
        if(param == null) throw new ScripterRuntimeException($"StringChooserParam {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        param.val = args[3].ToString();
        return args[3];
    }

    private static Value InvokeTrigger(RuntimeDomain domain, Value[] args)
    {
        Reference.ValidateArgumentsLength(nameof(InvokeTrigger), args, 3);
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].AsString;
        var param = storable.GetAction(paramName);
        if(param == null) throw new ScripterRuntimeException($"Action {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        param.actionCallback.Invoke();
        return Value.Void;
    }

    private static Value InvokeKeybinding(RuntimeDomain domain, Value[] args)
    {
        throw new NotImplementedException();
    }

    private static JSONStorable GetStorable(Value atomName, Value storableName)
    {
        var atom = SuperController.singleton.GetAtomByUid(atomName.AsString);
        if (atom == null) throw new ScripterPluginException($"Could not find an atom named '{atomName}'");
        var storable = atom.GetStorableByID(storableName.AsString);
        if (storable == null) throw new ScripterPluginException($"Could not find an storable named '{storableName}' in atom '{atomName}'");
        return storable;
    }
}
