using ScripterLang;

public class AudioActionReference : ObjectReference
{
    private readonly JSONStorableActionAudioClip _param;

    public AudioActionReference(JSONStorableActionAudioClip param)
    {
        _param = param;
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
        _param.actionCallback(nac.Nac);
        return Value.Void;
    }
}
