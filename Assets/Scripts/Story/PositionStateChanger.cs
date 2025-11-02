using UnityEngine;

public class PositionStateChanger : MonoBehaviour
{
    [SerializeField] private Vector3[] positions;
    [SerializeField] private IntVariable storyVariable;
    private void OnEnable()
    {
        storyVariable.OnValueChanged += ChangePosition;
    }
    private void OnDisable()
    {
        storyVariable.OnValueChanged -= ChangePosition;
    }
    public void ChangePosition(int newState)
    {
        transform.position = positions[newState];
    }
}