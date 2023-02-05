using System;
using System.Collections;
using ScripterLang;
using SimpleJSON;
using UnityEngine;

public class ScriptUpdateTrigger : ScriptTrigger
{
    public const string Type = "Update";

    public override string GetTypeName() => Type;

    private readonly Action<Value> _run;
    private Coroutine _co;

    public ScriptUpdateTrigger(string name, Action<Value> run, bool enabled, MVRScript plugin)
        : base(name, enabled, plugin)
    {
        _run = run;
    }

    public static ScriptTrigger FromJSONImpl(JSONNode json, Action<Value> run, MVRScript plugin)
    {
        return new ScriptUpdateTrigger(
            json["Name"],
            run,
            json["Enabled"].AsBool,
            plugin
        );
    }

    private IEnumerator Update()
    {
        while (true)
        {
            yield return 0;
            if (!EnabledJSON.val) continue;
            _run(Value.Undefined);
        }
        // ReSharper disable once IteratorNeverReturns
    }

    public override void Register()
    {
        _co = Plugin.StartCoroutine(Update());
    }

    public override void Deregister()
    {
        Plugin.StopCoroutine(_co);
    }
}
