﻿using System.Collections.Generic;
using ScripterLang;

public class StorableReference : ObjectReference
{
    private readonly JSONStorable _storable;

    public StorableReference(JSONStorable storable)
    {
        _storable = storable;
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "getAllParamNames":
                return Func(GetAllParamNames);
            case "invokeAction":
                return Func(InvokeAction);
            case "getAudioAction":
                return Func(GetAudioAction);
            case "getFloat":
                return Func(GetFloat);
            case "getString":
                return Func(GetString);
            case "getStringChooser":
                return Func(GetStringChooser);
            case "getUrl":
                return Func(GetUrl);
            case "getBool":
                return Func(GetBool);
            default:
                return base.GetProperty(name);
        }
    }

    private Value GetAllParamNames(LexicalContext context, Value[] args)
    {

        var raw = _storable.GetAllParamAndActionNames();
        var values = new List<Value>(raw.Count);
        for (var i = 0; i < raw.Count; i++)
        {
            values[i] = raw[i];
        }
        return new ListReference(values);
    }

    public Value InvokeAction(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(InvokeAction), args, 1);
        var paramName = args[0].AsString;
        _storable.CallAction(paramName);
        return Value.Void;
    }

    public Value GetAudioAction(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetAudioAction), args, 1);
        var paramName = args[0].AsString;
        var action = _storable.GetAudioClipAction(paramName);
        if(action == null) throw new ScripterRuntimeException($"Could not find an audio clip action named {paramName} in storable {_storable.storeId} in atom {_storable.containingAtom.storeId}.");
        return new AudioActionReference(action);
    }

    public Value GetFloat(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetFloat), args, 1);
        var paramName = args[0].AsString;
        var param = _storable.GetFloatJSONParam(paramName);
        if (param == null) throw new ScripterPluginException($"Could not find a float param named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
        return new FloatParamReference(param);
    }

    public Value GetString(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetString), args, 1);
        var paramName = args[0].AsString;
        var param = _storable.GetStringJSONParam(paramName);
        if (param == null) throw new ScripterPluginException($"Could not find a string named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
        return new StringParamReference(param);
    }

    public Value GetStringChooser(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetStringChooser), args, 1);
        var paramName = args[0].AsString;
        var param = _storable.GetStringChooserJSONParam(paramName);
        if (param == null) throw new ScripterPluginException($"Could not find a string chooser param named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
        return new StringChooserParamReference(param);
    }

    public Value GetUrl(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetStringChooser), args, 1);
        var paramName = args[0].AsString;
        var param = _storable.GetUrlJSONParam(paramName);
        if (param == null) throw new ScripterPluginException($"Could not find a url param named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
        return new UrlParamReference(param);
    }

    public Value GetBool(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetBool), args, 1);
        var paramName = args[0].AsString;
        var param = _storable.GetBoolJSONParam(paramName);
        if (param == null) throw new ScripterPluginException($"Could not find a bool param named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
        return new BoolParamReference(param);
    }
}
