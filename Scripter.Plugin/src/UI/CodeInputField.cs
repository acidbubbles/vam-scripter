using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CodeInputField : InputField
{
    private int _prevCaretPosition;
    private bool _lastSelected;
    private int _lastSelectedPosition;

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

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (_lastSelected)
        {
            caretPosition = _lastSelectedPosition;
            ForceLabelUpdate();
            _lastSelected = false;
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
