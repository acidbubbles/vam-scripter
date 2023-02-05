using System;
using ScripterLang;
using UnityEngine;

public static class VamFunctions
{
    public static void Register(GlobalLexicalContext lexicalContext)
    {
        lexicalContext.Functions.Add("getDateTime", GetDateTime);
        lexicalContext.Functions.Add("logMessage", LogMessage);
        lexicalContext.Functions.Add("logError", LogError);
        lexicalContext.Functions.Add("getTime", _ => Time.time);
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

    private static Value GetDateTime(Value[] args)
    {
        var now = DateTime.Now;
        if (args.Length == 0)
            return now.ToString("s");
        else
            return now.ToString(args[0].StringValue);
    }

    private static Value LogMessage(Value[] args)
    {
        ValidateArgumentsLength(args, 1, nameof(LogMessage));
        SuperController.LogMessage("Scripter: " + args[0].StringValue);
        return Value.Undefined;
    }

    private static Value LogError(Value[] args)
    {
        ValidateArgumentsLength(args, 1, nameof(LogError));
        SuperController.LogError(args[0].ToString());
        return Value.Undefined;
    }

    private static Value GetFloatParamValue(Value[] args)
    {
        ValidateArgumentsLength(args, 3, nameof(GetFloatParamValue));
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].StringValue;
        var param = storable.GetFloatJSONParam(paramName);
        if(param == null) throw new ScripterRuntimeException($"FloatParam {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        return Value.CreateFloat(param.val);
    }

    private static Value SetFloatParamValue(Value[] args)
    {
        ValidateArgumentsLength(args, 4, nameof(SetFloatParamValue));
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].StringValue;
        var param = storable.GetFloatJSONParam(paramName);
        if(param == null) throw new ScripterRuntimeException($"FloatParam {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        param.val = args[3].FloatValue;
        return args[3];
    }

    private static Value GetBoolParamValue(Value[] args)
    {
        ValidateArgumentsLength(args, 3, nameof(GetBoolParamValue));
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].StringValue;
        var param = storable.GetBoolJSONParam(paramName);
        if(param == null) throw new ScripterRuntimeException($"BoolParam {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        return Value.CreateBoolean(param.val);
    }

    private static Value SetBoolParamValue(Value[] args)
    {
        ValidateArgumentsLength(args, 4, nameof(SetBoolParamValue));
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].StringValue;
        var param = storable.GetBoolJSONParam(paramName);
        if(param == null) throw new ScripterRuntimeException($"BoolParam {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        param.val = args[3].AsBool;
        return args[3];
    }

    private static Value GetStringParamValue(Value[] args)
    {
        ValidateArgumentsLength(args, 3, nameof(GetStringParamValue));
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].StringValue;
        var param = storable.GetStringJSONParam(paramName);
        if(param == null) throw new ScripterRuntimeException($"StringParam {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        return Value.CreateString(param.val);
    }

    private static Value SetStringParamValue(Value[] args)
    {
        ValidateArgumentsLength(args, 4, nameof(SetStringParamValue));
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].StringValue;
        var param = storable.GetStringJSONParam(paramName);
        if(param == null) throw new ScripterRuntimeException($"StringParam {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        param.val = args[3].ToString();
        // return args[3];
        return Value.Undefined;
    }

    private static Value GetStringChooserParamValue(Value[] args)
    {
        ValidateArgumentsLength(args, 3, nameof(GetStringChooserParamValue));
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].StringValue;
        var param = storable.GetStringChooserJSONParam(paramName);
        if(param == null) throw new ScripterRuntimeException($"StringChooserParam {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        return Value.CreateString(param.val);
    }

    private static Value SetStringChooserParamValue(Value[] args)
    {
        ValidateArgumentsLength(args, 4, nameof(SetStringChooserParamValue));
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].StringValue;
        var param = storable.GetStringChooserJSONParam(paramName);
        if(param == null) throw new ScripterRuntimeException($"StringChooserParam {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        param.val = args[3].ToString();
        return args[3];
    }

    private static Value InvokeTrigger(Value[] args)
    {
        ValidateArgumentsLength(args, 3, nameof(InvokeTrigger));
        var storable = GetStorable(args[0], args[1]);
        var paramName = args[2].StringValue;
        var param = storable.GetAction(paramName);
        if(param == null) throw new ScripterRuntimeException($"Action {paramName} was not found in storable {storable.storeId} of atom {storable.containingAtom.uid}");
        param.actionCallback.Invoke();
        return Value.Undefined;
    }

    private static Value InvokeKeybinding(Value[] args)
    {
        throw new NotImplementedException();
    }

    private static JSONStorable GetStorable(Value atomName, Value storableName)
    {
        var atom = SuperController.singleton.GetAtomByUid(atomName.StringValue);
        if (atom == null) throw new ScripterPluginException($"Could not find an atom named '{atomName}'");
        var storable = atom.GetStorableByID(storableName.StringValue);
        if (storable == null) throw new ScripterPluginException($"Could not find an storable named '{storableName}' in atom '{atomName}'");
        return storable;
    }

    private static void ValidateArgumentsLength(Value[] args, int expectedLength, string fnName)
    {
        if (args.Length < expectedLength) throw new ScripterRuntimeException($"Method {fnName[0].ToString().ToLower()}{fnName.Substring(1)} Expected {expectedLength} arguments, received {args.Length}");
    }
}
