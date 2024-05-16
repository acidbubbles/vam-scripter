using System.Collections.Generic;
using System.Linq;
using ScripterLang;

public class StorableProxy
{
    private readonly Atom _atom;
    private string _storableName;
    private bool _hasFailedOnce;

    public StorableProxy(Atom atom, string storableName)
    {
        _atom = atom;
        _storableName = storableName;
    }

    public JSONStorable GetStorable()
    {
        var storable = _atom.GetStorableByID(_storableName);
        if (storable == null)
        {
            var completeStorableName = _atom.GetStorableIDs().FirstOrDefault(s => s.EndsWith(_storableName));
            if (completeStorableName == null)
            {
                if (!_hasFailedOnce)
                {
                    SuperController.LogError($"Scripter: Could not find a storable named or ending with '{_storableName}' in atom '{_atom.name}'. Will retry until the storable is found.");
                    _hasFailedOnce = true;
                }
                return null;
            }
            storable = _atom.GetStorableByID(completeStorableName);
            if (storable == null)
                throw new ScripterPluginException($"Found but unable to get storable '{completeStorableName}' (from '{_storableName}') in atom '{_atom.name}'");
            _storableName = completeStorableName;
        }
        else if (_hasFailedOnce)
        {
            SuperController.LogMessage("Scripter: Found storable '{_storableName}' in atom '{_atom.name}' after a previous failure.");
            _hasFailedOnce = false;
        }

        return storable;
    }

    public string GetDisplayName()
    {
        return _atom.name + "/" + _storableName;
    }
}

public class StorableReference : ObjectReference
{
    private readonly StorableProxy _storableProxy;

    public StorableReference(Atom atom, string storableName)
    {
        _storableProxy = new StorableProxy(atom, storableName);
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
        var storable = _storableProxy.GetStorable();
        if (storable == null) return Value.Undefined;
        var raw = storable.GetAllParamAndActionNames();
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
        var storable = _storableProxy.GetStorable();
        if (storable == null)
            throw new ScripterRuntimeException($"Could not invoke action {paramName} because storable {_storableProxy.GetDisplayName()} cannot be found.");
        storable.CallAction(paramName);
        return Value.Void;
    }

    private Value GetAudioClipAction(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetAudioClipAction), args, 1);
        var paramName = args[0].AsString;
        if(_storableProxy.GetStorable()?.GetAudioClipAction(paramName) == null) throw new ScripterRuntimeException($"Could not find an audio clip action named {paramName} in storable {_storableProxy.GetDisplayName()}.");
        return new AudioActionReference(_storableProxy, paramName);
    }

    private Value GetFloatParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetFloatParam), args, 1);
        var paramName = args[0].AsString;
        var storable = _storableProxy.GetStorable();
        if (storable != null)
        {
            if (storable.GetFloatJSONParam(paramName) == null) throw new ScripterPluginException($"Could not find a float param named {paramName} in storable {_storableProxy.GetDisplayName()}");
        }
        return new FloatParamReference(_storableProxy, paramName);
    }

    private Value GetStringParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetStringParam), args, 1);
        var paramName = args[0].AsString;
        var storable = _storableProxy.GetStorable();
        if (storable != null)
        {
            if (storable.GetStringJSONParam(paramName) == null) throw new ScripterPluginException($"Could not find a string named {paramName} in storable {_storableProxy.GetDisplayName()}");
        }
        return new StringParamReference(_storableProxy, paramName);
    }

    private Value GetStringChooserParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetStringChooserParam), args, 1);
        var paramName = args[0].AsString;
        var storable = _storableProxy.GetStorable();
        if (storable != null)
        {
            if (storable.GetStringChooserJSONParam(paramName) == null) throw new ScripterPluginException($"Could not find a string chooser param named {paramName} in storable {_storableProxy.GetDisplayName()}");
        }
        return new StringChooserParamReference(_storableProxy, paramName);
    }

    private Value GetUrlParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetUrlParam), args, 1);
        var paramName = args[0].AsString;
        var storable = _storableProxy.GetStorable();
        if (storable != null)
        {
            if (storable.GetUrlJSONParam(paramName) == null) throw new ScripterPluginException($"Could not find a url param named {paramName} in storable {_storableProxy.GetDisplayName()}");
        }
        return new UrlParamReference(_storableProxy, paramName);
    }

    private Value GetBoolParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetBoolParam), args, 1);
        var paramName = args[0].AsString;
        var storable = _storableProxy.GetStorable();
        if (storable != null)
        {
            if (storable.GetBoolJSONParam(paramName) == null) throw new ScripterPluginException($"Could not find a bool param named {paramName} in storable {_storableProxy.GetDisplayName()}");
        }
        return new BoolParamReference(_storableProxy, paramName);
    }

    private Value GetColorParam(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetColorParam), args, 1);
        var paramName = args[0].AsString;
        var storable = _storableProxy.GetStorable();
        if (storable != null)
        {
            if (storable.GetColorJSONParam(paramName) == null) throw new ScripterPluginException($"Could not find a color param named {paramName} in storable {_storableProxy.GetDisplayName()}");
        }
        return new ColorParamReference(_storableProxy, paramName);
    }
}
