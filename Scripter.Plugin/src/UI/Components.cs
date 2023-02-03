using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public static class Components
    {
        public static UIDynamicTextField MakeMultilineInput(UIDynamicTextField textfield, JSONStorableString jss)
        {
            textfield.height = 840;
            jss.dynamicText = textfield;
            textfield.backgroundColor = Color.white;
            SuperController.singleton.transform.parent.BroadcastMessage("DevToolsGameObjectExplorerShow", textfield.gameObject);
            var text = textfield.GetComponentInChildren<Text>(true);
            var input = text.gameObject.AddComponent<InputField>();
            input.lineType = InputField.LineType.MultiLineNewline;
            input.textComponent = textfield.UItext;
            jss.inputField = input;
            textfield.gameObject.AddComponent<Clickable>().onClick.AddListener(_ =>
            {
                EventSystem.current.SetSelectedGameObject(input.gameObject, null);
                input.ActivateInputField();
                input.StartCoroutine(Select(input));
            });
            return textfield;
        }

        private static IEnumerator Select(InputField input)
        {
            yield return 0;
            input.MoveTextEnd(true);
        }

        public static Transform MakeToolbar(UIDynamic spacer)
        {
            spacer.height = 40;
            var layout = spacer.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 4f;
            layout.childForceExpandWidth = false;
            layout.childControlWidth = true;
            return layout.transform;
        }

        public static void AddToToolbar(Transform toolbar, Transform prefab, string label, UnityAction action)
        {
            var button = Object.Instantiate(prefab, toolbar, false);
            var ui = button.GetComponent<UIDynamicButton>();
            ui.label = label;
            ui.button.onClick.AddListener(action);
            var layoutElement = button.GetComponent<LayoutElement>();
            layoutElement.preferredWidth = 0;
            layoutElement.flexibleWidth = 100;
        }
    }
}
