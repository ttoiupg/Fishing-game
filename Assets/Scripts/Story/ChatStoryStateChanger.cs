using UnityEngine;

public class ChatStoryStateChanger : MonoBehaviour
{
    [SerializeField] private ChatInteract chatInteract;
    [SerializeField] private DialogueSection[] sections;
    [SerializeField] private IntVariable storyVariable;
    private void OnEnable()
    {
        storyVariable.OnValueChanged += ChangeState;
    }
    private void Start() {
        chatInteract.startSection = sections[storyVariable.Value];
    }
    private void OnDisable()
    {
        storyVariable.OnValueChanged -= ChangeState;
    }
    public void ChangeState(int newState)
    {
        chatInteract.startSection = sections[newState];
    }
}