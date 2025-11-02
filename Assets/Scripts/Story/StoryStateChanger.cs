using NUnit.Framework;
using UnityEngine;

public class SpriteStoryStateChanger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] stateSprites;
    [SerializeField] private IntVariable storyVariable;

    private void OnEnable()
    {
        storyVariable.OnValueChanged += ChangeState;
    }
    private void OnDisable()
    {
        storyVariable.OnValueChanged -= ChangeState;
    }
    public void ChangeState(int newState)
    {
        spriteRenderer.sprite = stateSprites[newState];
    }
}
