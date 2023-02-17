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
        onValidateInput = OnDirectValidateInput;
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

    private bool _ignoreOnValidateInput;
    private char OnDirectValidateInput(string inputText, int pos, char input)
    {
        if (_ignoreOnValidateInput) return input;
        if (input == '\t')
        {
            if (selectionAnchorPosition != selectionFocusPosition)
            {
                // Change indentation
                var before = inputText.Substring(0, selectionAnchorPosition);
                var selected = inputText.Substring(selectionAnchorPosition, selectionFocusPosition - selectionAnchorPosition);
                var after = inputText.Substring(selectionFocusPosition, inputText.Length - selectionFocusPosition);
                selected = Input.GetKey(KeyCode.LeftShift)
                    ? selected.Replace("\n  ", "\n")
                    : selected.Replace("\n", "\n  ");
                _ignoreOnValidateInput = true;
                text = before + selected + after;
                _ignoreOnValidateInput = false;
                selectionAnchorPosition = before.Length;
                selectionFocusPosition = before.Length + selected.Length;
            }
            else
            {
                // Insert two spaces
                _ignoreOnValidateInput = true;
                text = inputText.Insert(pos, "  ");
                _ignoreOnValidateInput = false;
                caretPosition = pos + 2;
            }
            return '\0';
        }
        if (input == '\n')
        {
            if (pos <= 0) return input;
            var startOfLine = FindStartOfLine(pos);
            var line = inputText.Substring(startOfLine, pos - startOfLine);
            // Find the indentation of line
            var indent = 0;
            while (indent < line.Length && line[indent] == ' ')
                indent++;
            if(line.Length > 0 && line[line.Length - 1] == '{')
                indent += 2;
            var newLineIndent = "\n" + new string(' ', indent);

            _ignoreOnValidateInput = true;
            text = inputText.Insert(pos, newLineIndent);
            _ignoreOnValidateInput = false;
            caretPosition = pos + newLineIndent.Length;
            return '\0';
        }
        if (input == '}')
        {
            // De-indent
            if (pos == 0) return input;
            var startOfLine = FindStartOfLine(pos);
            var line = inputText.Substring(startOfLine, pos - startOfLine);
            var whitespace = true;
            for (var i = 0; i < line.Length; i++)
            {
                if (line[i] != ' ')
                {
                    whitespace = false;
                    break;
                }
            }
            if (whitespace && line.Length >= 2)
            {
                _ignoreOnValidateInput = true;
                text = inputText.Remove(pos - 2, 2);
                _ignoreOnValidateInput = false;
                caretPosition = pos - 2;
            }
        }
        return input;
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
