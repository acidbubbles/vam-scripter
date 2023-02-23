using System.Collections.Generic;
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
            case "getAudioClipAction":
                return Func(GetAudioClipAction);
            case "getFloatParam":
                return Func(GetFloatParam);
            case "getStringParam":
                return Func(GetStringParam);
            case "getStringChooserParam":
                return Func(GetStringChooserParam);
            case "getUrlParam":
                return Func(GetUrlParam);
            case "getBoolParam":
                return Func(GetBoolParam);
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

    private Value InvokeAction(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(InvokeAction), args, 1);
        var paramName = args[0].AsString;
        _storable.CallAction(paramName);
        return Value.Void;
    }

    private Value GetAudioClipAction(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetAudioClipAction), args, 1);
        var paramName = args[0].AsString;
        var action = _storable.GetAudioClipAction(paramName);
        if(action == null) throw new ScripterRuntimeException($"Could not find an audio clip action named {paramName} in storable {_storable.storeId} in atom {_storable.containingAtom.storeId}.");
        return new AudioActionReference(action);
    }

    private Value GetFloatParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetFloatParam), args, 1);
        var paramName = args[0].AsString;
        var param = _storable.GetFloatJSONParam(paramName);
        if (param == null) throw new ScripterPluginException($"Could not find a float param named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
        return new FloatParamReference(param);
    }

    private Value GetStringParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetStringParam), args, 1);
        var paramName = args[0].AsString;
        var param = _storable.GetStringJSONParam(paramName);
        if (param == null) throw new ScripterPluginException($"Could not find a string named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
        return new StringParamReference(param);
    }

    private Value GetStringChooserParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetStringChooserParam), args, 1);
        var paramName = args[0].AsString;
        var param = _storable.GetStringChooserJSONParam(paramName);
        if (param == null) throw new ScripterPluginException($"Could not find a string chooser param named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
        return new StringChooserParamReference(param);
    }

    private Value GetUrlParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetStringChooserParam), args, 1);
        var paramName = args[0].AsString;
        var param = _storable.GetUrlJSONParam(paramName);
        if (param == null) throw new ScripterPluginException($"Could not find a url param named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
        return new UrlParamReference(param);
    }

    private Value GetBoolParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetBoolParam), args, 1);
        var paramName = args[0].AsString;
        var param = _storable.GetBoolJSONParam(paramName);
        if (param == null) throw new ScripterPluginException($"Could not find a bool param named {paramName} in storable '{_storable.storeId}' in atom '{_storable.containingAtom.storeId}'");
        return new BoolParamReference(param);
    }
}
