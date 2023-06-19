using System;
using System.Security.Cryptography;
using MVR.FileManagementSecure;
using ScripterLang;

public class FileSystemReference : ObjectReference
{
    private const string _scripterDirectory = "Saves\\PluginData\\Scripter";

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "writeSceneFileSync":
                return Func(WriteSceneFileSync);
            case "readSceneFileSync":
                return Func(ReadSceneFileSync);
            case "unlinkSceneFileSync":
                return Func(UnlinkSceneFileSync);
            default:
                return base.GetProperty(name);
        }
    }

    private Value WriteSceneFileSync(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(WriteSceneFileSync), args, 2);
        var path = GetScenePath(args[0].AsString);
        var content = args[1].AsString;
        FileManagerSecure.WriteAllText(path, content);
        return Value.Void;
    }

    private Value ReadSceneFileSync(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(ReadSceneFileSync), args, 1);
        var path = GetScenePath(args[0].AsString);
        return FileManagerSecure.ReadAllText(path);
    }

    private Value UnlinkSceneFileSync(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(UnlinkSceneFileSync), args, 1);
        var path = GetScenePath(args[0].AsString);
        FileManagerSecure.DeleteFile(path);
        return Value.Void;
    }

    private string GetScenePath(string path)
    {
        if(path.Contains("/") || path.Contains("\\") || path.Contains(".."))
            throw new ScripterRuntimeException("Invalid path: " + path);
        if(!path.EndsWith(".txt") || !path.EndsWith(".json"))
            throw new ScripterRuntimeException("Invalid path extension: " + path);
        var sceneDirectory = _scripterDirectory + "\\" + GetSceneIdentifier();
        FileManagerSecure.CreateDirectory(sceneDirectory);
        return sceneDirectory + "\\" + path;
    }

    private string GetSceneIdentifier()
    {
        if (Scripter.singleton.sceneIdentifier != null) return Scripter.singleton.sceneIdentifier;
        var id = CreateCryptographicallySecureGuid();
        Scripter.singleton.sceneIdentifier = id;
        return id;
    }

    private static string CreateCryptographicallySecureGuid()
    {
        var rng = new RNGCryptoServiceProvider();
        var key = new byte[16];
        rng.GetBytes(key);
        return Convert.ToBase64String(key);
    }
}
