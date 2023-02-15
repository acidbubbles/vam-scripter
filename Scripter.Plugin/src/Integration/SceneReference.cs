﻿using ScripterLang;

public class SceneReference : ObjectReference
{
    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "getAtom":
                return Func(GetAtom);
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
}
