using ScripterLang;

public class NamedAudioClipReference : ObjectReference
{
    public readonly NamedAudioClip nac;

    public NamedAudioClipReference(NamedAudioClip nac)
    {
        this.nac = nac;
    }
}
