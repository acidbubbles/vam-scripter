using System.Collections.Generic;
using System.Linq;
using ScripterLang;

public class SceneReference : ObjectReference
{
    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "getAtom":
                return Func(GetAtom);
            case "getAtoms":
                return Func(GetAtoms);
            case "getAtomIds":
                return Func(GetAtomIds);
            case "getAudioClip":
                return Func(GetAudioClip);
            // TODO: Find the player camera (look at eyes, for example)
            // TODO: Create Transform object (a target that the user can move and look at)
            default:
                return base.GetProperty(name);
        }
    }

    private static Value GetAtom(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetAtom), args, 1);
        var atomName = args[0].AsString;
        var atom = SuperController.singleton.GetAtomByUid(atomName);
        if (atom == null) throw new ScripterPluginException($"Could not find an atom named '{atomName}'");
        return new AtomReference(atom);
    }

    private static Value GetAtoms(LexicalContext context, Value[] args)
    {
        var raw = SuperController.singleton.GetAtoms();
        var values = new List<Value>(raw.Count);
        for (var i = 0; i < raw.Count; i++)
        {
            values[i] = new AtomReference(raw[i]);
        }
        return new ListReference(values);
    }

    private static Value GetAtomIds(LexicalContext context, Value[] args)
    {
        var raw = SuperController.singleton.GetAtomUIDs();
        var values = new List<Value>(raw.Count);
        for (var i = 0; i < raw.Count; i++)
        {
            values[i] = raw[i];
        }
        return new ListReference(values);
    }

    private static Value GetAudioClip(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(GetAtom), args, 3);
        var type = args[0].AsString;
        var category = args[1].AsString;
        var clip = args[2].AsString;
        List<NamedAudioClip> list;
        if (type == "Embedded")
            list = EmbeddedAudioClipManager.singleton.GetCategoryClips(category);
        else if (type == "URL")
            list = URLAudioClipManager.singleton.GetCategoryClips(category);
        else
            throw new ScripterRuntimeException("Invalid audio clip type. Must be 'Embedded' or 'URL'");
        if (list == null)
            throw new ScripterRuntimeException("Invalid audio clip category.");
        var nac = list.FirstOrDefault(x => x.displayName == clip);
        return new NamedAudioClipReference(nac);
    }
}
