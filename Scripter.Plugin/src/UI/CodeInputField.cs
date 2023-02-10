using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CodeInputField : InputField
{
    private int _prevCaretPosition;
    private bool _lastSelected;
    private int _lastSelectedPosition;
    private bool _lastDoubleClick;
    private int _lastDoubleClickStart;
    private int _lastDoubleClickEnd;

    protected override void Awake()
    {
        base.Awake();
        lineType = LineType.MultiLineNewline;
    }

    public override void Rebuild(CanvasUpdate update)
    {
        base.Rebuild(update);
        _prevCaretPosition = caretPosition;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        var pointerData = eventData as PointerEventData;
        if (pointerData != null)
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(textComponent.rectTransform, pointerData.position, pointerData.pressEventCamera, out mousePos);
            _lastSelectedPosition = GetCharacterIndexFromPosition(mousePos);
        }
        else
        {
            _lastSelectedPosition = text.Length;
        }
        _lastSelected = true;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (eventData.clickCount == 2)
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(textComponent.rectTransform, eventData.position, eventData.pressEventCamera, out mousePos);
            var pos = GetCharacterIndexFromPosition(mousePos);
            if (pos == text.Length)
                pos--;
            var start = pos;
            while (start > 0)
            {
                var c = text[start - 1];
                if (c == '\r' || c == '\n' || c == ' ' || c == '.' || c == '"' || c == '(' || c == ')' || c == ';' || c == ',')
                    break;
                start--;
            }
            var end = pos;
            while (end < text.Length - 2)
            {
                var c = text[end];
                if (c == '\r' || c == '\n' || c == ' ' || c == '.' || c == '"' || c == '(' || c == ')' || c == ';' || c == ',')
                    break;
                end++;
            }

            _lastDoubleClick = true;
            _lastDoubleClickStart = start;
            _lastDoubleClickEnd = end;
        }
        else if (eventData.clickCount == 3)
        {
            _lastDoubleClick = true;
            _lastDoubleClickStart = 0;
            _lastDoubleClickEnd = text.Length;
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (_lastSelected)
        {
            caretPosition = _lastSelectedPosition;
            ForceLabelUpdate();
            _lastSelected = false;
        }
        else if (_lastDoubleClick)
        {
            caretPosition = _lastDoubleClickStart;
            selectionAnchorPosition = _lastDoubleClickStart;
            selectionFocusPosition = _lastDoubleClickEnd;
            _lastDoubleClick = false;
            ForceLabelUpdate();
        }
        else if (Input.GetKeyDown(KeyCode.End) && !Input.GetKey(KeyCode.LeftControl))
        {
            var newCaretPosition = FindEndOfLine(_prevCaretPosition);
            caretPosition = newCaretPosition;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                selectionAnchorPosition = _prevCaretPosition;
                selectionFocusPosition = newCaretPosition;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Home) && !Input.GetKey(KeyCode.LeftControl))
        {
            var newCaretPosition = FindStartOfLine(_prevCaretPosition);
            caretPosition = newCaretPosition;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                selectionAnchorPosition = _prevCaretPosition;
                selectionFocusPosition = newCaretPosition;
            }
        }
    }

    private int FindStartOfLine(int startIndex)
    {
        var index = text.LastIndexOf('\n', startIndex - 1);
        if (index == -1) return text.Length;
        return index + 1;
    }

    private int FindEndOfLine(int startIndex)
    {
        var index = text.IndexOf('\n', startIndex);
        if (index == -1) return text.Length;
        return index;
    }
}
