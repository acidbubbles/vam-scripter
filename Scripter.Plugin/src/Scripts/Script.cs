﻿using System;
using SimpleJSON;
using UnityEngine;

public class Script
{
    private readonly Scripter _scripter;

    public CodeInputField input;
    public ScripterTab tab;

    public bool dirty;

    public readonly HistoryManager history;
    public readonly JSONStorableString nameJSON = new JSONStorableString("Module", "");
    public readonly JSONStorableString sourceJSON = new JSONStorableString("Source", "");

    public Script(string moduleName, string source, Scripter scripter)
    {
        _scripter = scripter;

        history = new HistoryManager(sourceJSON);

        nameJSON.val = moduleName;
        nameJSON.setCallbackFunction = val =>
        {
            scripter.programFiles.Unregister(this);
            Parse();
        };

        sourceJSON.setCallbackFunction = val =>
        {
            dirty = true;
            history.Update(val);
        };
        sourceJSON.valNoCallback = source;
        if (!string.IsNullOrEmpty(source))
            dirty = true;
    }

    private int _lastParsed;

    public void Parse()
    {
        // To prevent clicking on Validate running the script twice
        if (_lastParsed == Time.frameCount) return;
        _lastParsed = Time.frameCount;
        Scripter.singleton.SaveToDisk(this);
        Parse(sourceJSON.val);
    }

    private void Parse(string val)
    {
        try
        {
            _scripter.programFiles.RegisterFile(nameJSON.val, val);
            dirty = false;
            if (_scripter.isLoading) return;
            _scripter.console.Log($"<color=green>Parsed `{nameJSON.val}` successfully</color>");
        }
        catch (Exception exc)
        {
            _scripter.console.LogError($"{nameJSON.val} failed to compile: {exc.Message}");
        }
    }

    public static Script FromJSON(JSONNode json, Scripter plugin)
    {
        var s = new Script(json["Module"].Value, json["Source"].Value, plugin);
        return s;
    }

    public JSONNode GetJSON()
    {
        return new JSONClass
        {
            { "Module", nameJSON.val },
            { "Source", sourceJSON.val },
        };
    }
}
