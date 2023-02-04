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
        layout.preferredHeight = 220;

        var colGroup = go.AddComponent<VerticalLayoutGroup>();
        colGroup.spacing = 10f;
        colGroup.childControlHeight = true;
        colGroup.childForceExpandHeight = false;

        go.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;

        var bg = go.AddComponent<Image>();
        bg.color = new Color(0.721f, 0.682f, 0.741f);
        bg.raycastTarget = false;

        {
            var title = new GameObject();
            title.transform.SetParent(go.transform, false);
            var titleLayout = title.AddComponent<LayoutElement>();
            titleLayout.minHeight = 40;

            var text = title.gameObject.AddComponent<Text>();
            text.text = "Trigger";
            text.font = manager.Prefabs.configurableButtonPrefab.GetComponent<UIDynamicButton>().buttonText.font;
            text.fontSize = 28;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.black;
        }

        {
            var rowGo = new GameObject();
            rowGo.transform.SetParent(go.transform, false);

            var rowGroup = rowGo.AddComponent<HorizontalLayoutGroup>();
            rowGroup.spacing = 10f;
            rowGroup.childControlWidth = true;
            rowGroup.childForceExpandWidth = true;

            var input = CreateTextInput(script.NameJSON, manager.Prefabs.configurableTextFieldPrefab, rowGo.transform);
            var inputLayout = input.GetComponent<LayoutElement>();
            inputLayout.flexibleWidth = 1000;
            inputLayout.minWidth = 800;
            inputLayout.preferredWidth = 800;

            var edit = Instantiate(manager.Prefabs.configurableButtonPrefab, rowGo.transform).GetComponent<UIDynamicButton>();
            edit.label = "Edit";
            edit.button.onClick.AddListener(() => { manager.EditScript(script); });
            var editLayout = edit.GetComponent<LayoutElement>();
            editLayout.flexibleWidth = 1;
            editLayout.minWidth = 1;
            editLayout.preferredWidth = 1;

            var delete = Instantiate(manager.Prefabs.configurableButtonPrefab, rowGo.transform).GetComponent<UIDynamicButton>();
            delete.label = "Delete";
            delete.button.onClick.AddListener(() =>
            {
                if (delete.label == "Are you sure?")
                    manager.Scripts.Delete(script);
                else
                    delete.label = "Are you sure?";
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
            console.height = 60;
            console.backgroundColor = Color.black;
            console.textColor = Color.white;
            script.ConsoleJSON.dynamicText = console;
        }

        var entry = go.AddComponent<ScriptEntryComponent>();

        return entry;
    }

    private static UIDynamicTextField CreateTextInput(JSONStorableString jss, Transform configurableTextFieldPrefab, Transform container)
    {
        var textfield = Instantiate(configurableTextFieldPrefab).GetComponent<UIDynamicTextField>();
        textfield.gameObject.transform.SetParent(container, false);
        jss.dynamicText = textfield;

        textfield.backgroundColor = Color.white;

        var input = textfield.gameObject.AddComponent<InputField>();
        input.textComponent = textfield.UItext;
        jss.inputField = input;

        Destroy(textfield.GetComponent<LayoutElement>());

        var rect = textfield.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(0, -30f);
        rect.sizeDelta = new Vector2(0, 40f);
        return textfield;
    }
}
