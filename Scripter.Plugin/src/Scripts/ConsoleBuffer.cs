using System.Collections.Generic;

public class ConsoleBuffer
{
    private const int _maxLines = 100;

    public readonly JSONStorableString consoleJSON = new JSONStorableString("Console", "");
    private readonly List<int> _lines = new List<int>();

    public void Init(UIDynamicTextField textField)
    {
        consoleJSON.dynamicText = textField;
    }

    public void Log(string message)
    {
        if (_lines.Count == _maxLines)
        {
            var first = _lines[0];
            _lines.RemoveAt(0);
            consoleJSON.valNoCallback = consoleJSON.val.Substring(first);
        }
        consoleJSON.val += message + "\n";
        _lines.Add(message.Length + 1);
    }

    public void LogError(string message)
    {
        Log("<color=red>" + message + "</color>");
        if (!consoleJSON.text.isActiveAndEnabled)
        {
            SuperController.LogError("Scripter: " + message);
        }
    }

    public void Clear()
    {
        _lines.Clear();
        consoleJSON.val = "";
    }
}
