using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Values/Int Values")]
public class IntValue : RuntimeScriptableObject
{
    [SerializeField] int initialValue;
    [SerializeField] int value;

    public event UnityAction<int> OnValueChanged = delegate { };

    public int Value
    {
        get => value;
        set
        {
            if (this.value == value) return;
            this.value = value;
            OnValueChanged.Invoke(value);
        }
    }
    protected override void OnReset()
    {
        OnValueChanged.Invoke(value = initialValue);
    }
}