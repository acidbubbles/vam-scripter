using ScripterLang;

public class AudioActionReference : ObjectReference
{
    private readonly StorableProxy _storableRef;
    private readonly string _paramName;

    public AudioActionReference(StorableProxy storableRef, string paramName)
    {
        _storableRef = storableRef;
        _paramName = paramName;
    }

    private JSONStorableActionAudioClip GetParam()
    {
        var storable = _storableRef.GetStorable();
        var param = storable.GetAudioClipAction(_paramName);
        if (param == null) throw new ScripterRuntimeException($"Bool param {_paramName} not found in {storable.name} of atom {storable.containingAtom.storeId}");
        return param;
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "play":
                return Func(Play);
            default:
                return base.GetProperty(name);
        }
    }

    public Value Play(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(Play), args, 1);
        var nac = args[0].AsObject as NamedAudioClipReference;
        if(nac == null) throw new ScripterRuntimeException($"Expected a NamedAudioClip in {nameof(Play)}");
        GetParam().actionCallback(nac.nac);
        return Value.Void;
    }
}
