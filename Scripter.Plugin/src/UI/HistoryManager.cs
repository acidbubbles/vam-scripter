using System.Collections.Generic;

public class HistoryManager
{
    private readonly JSONStorableString _storable;
    private readonly List<string> _history = new List<string>();
    private int _index;
    public UIDynamicButton undoButton;
    public UIDynamicButton redoButton;
    private bool _isNavigating;

    public HistoryManager(JSONStorableString storable)
    {
        _storable = storable;
        Update(_storable.val);
    }

    public void Update(string val)
    {
        if (_isNavigating) return;
        _isNavigating = true;
        try
        {
            while (_history.Count > _index + 1)
            {
                _history.RemoveAt(_history.Count - 1);
            }

            if (_history.Count > 50) _history.RemoveAt(0);
            _history.Add(val);
            _index = _history.Count - 1;
            UpdateButtons();
        }
        finally
        {
            _isNavigating = false;
        }
    }

    public void Undo()
    {
        _isNavigating = true;
        try
        {
            if (_index == 0) return;
            _index--;
            _storable.val = _history[_index];
            UpdateButtons();
        }
        finally
        {
            _isNavigating = false;
        }
    }

    public void Redo()
    {
        _isNavigating = true;
        try
        {
            if (_index >= _history.Count - 1) return;
            _index++;
            _storable.val = _history[_index];
            UpdateButtons();
        }
        finally
        {
            _isNavigating = false;
        }
    }

    public void UpdateButtons()
    {
        if (ReferenceEquals(undoButton, null)) return;
        undoButton.button.interactable = _index > 0;
        redoButton.button.interactable = _index < _history.Count - 1;
    }
}
