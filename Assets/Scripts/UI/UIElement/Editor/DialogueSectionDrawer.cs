using UnityEditor;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(DialogueData), true)]
public class DialogueSectionDrawer : PropertyDrawer
{
    public VisualTreeAsset dialogueDataTemplate;
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement inspector = new();
        dialogueDataTemplate.CloneTree(inspector);
        Label dialogueLabel = inspector.Q<Label>("Display");
        bool isShowing = true;
        Button Show = inspector.Q<Button>("ShowEffect");
        Show.clicked += () => {
            if (isShowing)
            {
                dialogueLabel.visible = false;
                isShowing = false;
            }
            else
            {
                dialogueLabel.visible = true;
                isShowing = true;
            }
        };
        return inspector;
    }
}