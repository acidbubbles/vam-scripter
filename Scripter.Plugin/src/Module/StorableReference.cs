using System.Collections.Generic;
using ScripterLang;

public class StorableReference : ObjectReference
{
    private readonly Atom _atom;
    private readonly string _storableName;

    public StorableReference(Atom atom, string storableName)
    {
        _atom = atom;
        _storableName = storableName;
    }

    public JSONStorable GetStorable()
    {
        var storable = _atom.GetStorableByID(_storableName);
        if (storable == null) throw new ScripterPluginException($"Could not find an storable named '{_storableName}' in atom '{_atom.name}'");
        return storable;
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
            case "getColorParam":
                return Func(GetColorParam);
            default:
                return base.GetProperty(name);
        }
    }

    private Value GetAllParamNames(LexicalContext context, Value[] args)
    {

        var raw = GetStorable().GetAllParamAndActionNames();
        var values = new List<Value>(raw.Count);
        for (var i = 0; i < raw.Count; i++)
        {
            values.Add(raw[i]);
        }
        return new ListReference(values);
    }

    private Value InvokeAction(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(InvokeAction), args, 1);
        var paramName = args[0].AsString;
        GetStorable().CallAction(paramName);
        return Value.Void;
    }

    private Value GetAudioClipAction(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetAudioClipAction), args, 1);
        var paramName = args[0].AsString;
        var action = GetStorable().GetAudioClipAction(paramName);
        if(action == null) throw new ScripterRuntimeException($"Could not find an audio clip action named {paramName} in storable {_storableName} in atom {_atom.storeId}.");
        return new AudioActionReference(this, paramName);
    }

    private Value GetFloatParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetFloatParam), args, 1);
        var paramName = args[0].AsString;
        var param = GetStorable().GetFloatJSONParam(paramName);
        if (param == null) throw new ScripterPluginException($"Could not find a float param named {paramName} in storable '{_storableName}' in atom '{_atom.storeId}'");
        return new FloatParamReference(this, paramName);
    }

    private Value GetStringParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetStringParam), args, 1);
        var paramName = args[0].AsString;
        var param = GetStorable().GetStringJSONParam(paramName);
        if (param == null) throw new ScripterPluginException($"Could not find a string named {paramName} in storable '{_storableName}' in atom '{_atom.storeId}'");
        return new StringParamReference(this, paramName);
    }

    private Value GetStringChooserParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetStringChooserParam), args, 1);
        var paramName = args[0].AsString;
        var param = GetStorable().GetStringChooserJSONParam(paramName);
        if (param == null) throw new ScripterPluginException($"Could not find a string chooser param named {paramName} in storable '{_storableName}' in atom '{_atom.storeId}'");
        return new StringChooserParamReference(this, paramName);
    }

    private Value GetUrlParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetUrlParam), args, 1);
        var paramName = args[0].AsString;
        var param = GetStorable().GetUrlJSONParam(paramName);
        if (param == null) throw new ScripterPluginException($"Could not find a url param named {paramName} in storable '{_storableName}' in atom '{_atom.storeId}'");
        return new UrlParamReference(this, paramName);
    }

    private Value GetBoolParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetBoolParam), args, 1);
        var paramName = args[0].AsString;
        var param = GetStorable().GetBoolJSONParam(paramName);
        if (param == null) throw new ScripterPluginException($"Could not find a bool param named {paramName} in storable '{_storableName}' in atom '{_atom.storeId}'");
        return new BoolParamReference(this, paramName);
    }

    private Value GetColorParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetColorParam), args, 1);
        var paramName = args[0].AsString;
        var param = GetStorable().GetColorJSONParam(paramName);
        if (param == null) throw new ScripterPluginException($"Could not find a color param named {paramName} in storable '{_storableName}' in atom '{_atom.storeId}'");
        return new ColorParamReference(this, paramName);
    }
}
