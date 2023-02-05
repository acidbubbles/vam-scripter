using UnityEngine;
using UnityEngine.UI;

public class ScriptEntryComponent : MonoBehaviour
{
    public static ScriptEntryComponent Create(Transform parent, ScreenManager manager, Script script)
    {
        var go = new GameObject();
        go.transform.SetParent(parent, false);

        var layout = go.AddComponent<LayoutElement>();
        layout.flexibleWidth = 1;
        layout.flexibleHeight = 0;
        layout.preferredHeight = 140;

        var colGroup = go.AddComponent<VerticalLayoutGroup>();
        colGroup.spacing = 10f;
        colGroup.childControlHeight = true;
        colGroup.childForceExpandHeight = false;

        go.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;

        var bg = go.AddComponent<Image>();
        bg.color = new Color(0.721f, 0.682f, 0.741f);
        bg.raycastTarget = false;

        // {
        //     var title = new GameObject();
        //     title.transform.SetParent(go.transform, false);
        //     var titleLayout = title.AddComponent<LayoutElement>();
        //     titleLayout.minHeight = 40;
        //
        //     var text = title.gameObject.AddComponent<Text>();
        //     text.text = "Trigger: " + script.Trigger.GetTypeName();
        //     text.font = manager.Prefabs.configurableButtonPrefab.GetComponent<UIDynamicButton>().buttonText.font;
        //     text.fontSize = 28;
        //     text.fontStyle = FontStyle.Bold;
        //     text.alignment = TextAnchor.MiddleCenter;
        //     text.color = Color.black;
        // }

        {
            var rowGo = new GameObject();
            rowGo.transform.SetParent(go.transform, false);

            var rowGroup = rowGo.AddComponent<HorizontalLayoutGroup>();
            rowGroup.spacing = 10f;
            rowGroup.childControlWidth = true;
            rowGroup.childForceExpandWidth = true;

            var toggle = Instantiate(manager.Prefabs.configurableTogglePrefab, rowGo.transform).GetComponent<UIDynamicToggle>();
            toggle.label = "On" + script.Trigger.GetTypeName();
            script.Trigger.EnabledJSON.toggle = toggle.toggle;
            var toggleLayout = toggle.GetComponent<LayoutElement>();
            toggleLayout.flexibleWidth = 1;
            toggleLayout.minWidth = 200;
            toggleLayout.preferredWidth = 200;

            var inputLayout = CreateTextInput(script.Trigger.NameJSON, manager.Prefabs.configurableTextFieldPrefab, rowGo.transform);
            inputLayout.flexibleWidth = 8;
            inputLayout.minWidth = 1;
            inputLayout.preferredWidth = 1;

            var edit = Instantiate(manager.Prefabs.configurableButtonPrefab, rowGo.transform).GetComponent<UIDynamicButton>();
            edit.label = "Edit";
            edit.button.onClick.AddListener(() => { manager.EditScript(script); });
            var editLayout = edit.GetComponent<LayoutElement>();
            editLayout.flexibleWidth = 2;
            editLayout.minWidth = 50;
            editLayout.preferredWidth = 50;

            var delete = Instantiate(manager.Prefabs.configurableButtonPrefab, rowGo.transform).GetComponent<UIDynamicButton>();
            delete.label = "X";
            delete.button.onClick.AddListener(() =>
            {
                if (delete.label == "OK")
                    manager.Scripts.Delete(script);
                else
                    delete.label = "OK";
            });
            delete.buttonColor = Color.red;
            delete.textColor = Color.white;
            var deleteLayout = delete.GetComponent<LayoutElement>();
            deleteLayout.flexibleWidth = 1;
            deleteLayout.minWidth = 1;
            deleteLayout.preferredWidth = 1;
        }

        {
            var console = Instantiate(manager.Prefabs.configurableTextFieldPrefab, go.transform).GetComponent<UIDynamicTextField>();
            console.GetComponent<LayoutElement>().minHeight = 56;
            console.height = 56;
            console.backgroundColor = Color.black;
            console.textColor = Color.white;
            script.ConsoleJSON.dynamicText = console;
        }

        var entry = go.AddComponent<ScriptEntryComponent>();

        return entry;
    }

    private static LayoutElement CreateTextInput(JSONStorableString jss, Transform configurableTextFieldPrefab, Transform container)
    {
        var go = new GameObject();
        go.transform.SetParent(container, false);

        var bg = go.AddComponent<Image>();
        bg.color = Color.white;

        var layout = go.AddComponent<LayoutElement>();

        {
            var box = new GameObject();
            box.transform.SetParent(go.transform, false);

            var rect = box.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(-10, 0);

            var text = box.AddComponent<Text>();
            text.font = configurableTextFieldPrefab.GetComponentInChildren<Text>().font;
            text.fontSize = 28;
            text.alignment = TextAnchor.MiddleLeft;
            text.color = Color.black;

            var input = box.AddComponent<InputField>();
            input.textComponent = text;

            input.text = jss.val;
            input.onValueChanged.AddListener(val => jss.val = val);
        }

        return layout;
    }
}
