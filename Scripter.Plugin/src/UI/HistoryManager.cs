using System.Collections.Generic;

public class HistoryManager
{
    private readonly JSONStorableString _storable;
    private readonly List<string> _history = new List<string>();
    private int _index;

    public HistoryManager(JSONStorableString storable)
    {
        _storable = storable;
        Update(_storable.val);
    }

    public void Update(string val)
    {
        while (_history.Count > _index + 1)
        {
            _history.RemoveAt(_history.Count - 1);
        }
        if (_history.Count > 50) _history.RemoveAt(0);
        _history.Add(val);
        _index = _history.Count - 1;
    }

    public void Undo()
    {
        if (_index == 0) return;
        _index--;
        _storable.valNoCallback = _history[_index];
    }

    public void Redo()
    {
        if (_index >= _history.Count - 1) return;
        _index++;
        _storable.valNoCallback = _history[_index];
    }
}
