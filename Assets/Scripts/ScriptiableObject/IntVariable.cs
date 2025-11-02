using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class RuntimeScriptableObject : ScriptableObject
{
    static readonly List<RuntimeScriptableObject> Instances = new();

    private void OnEnable() => Instances.Add(this);
    private void OnDisable() => Instances.Remove(this);

    protected abstract void OnReset();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetAllInstances()
    {
        foreach (var instance in Instances)
        {
            instance.OnReset();
        }
    }
}

public abstract class Variable<T> : RuntimeScriptableObject
{
    public string id;
    [SerializeField] protected T initialValue;
    [SerializeField] protected T value;

    public event UnityAction<T> OnValueChanged;

    public virtual T Value
    {
        get => value;
        set
        {
            if (Equals(this.value, value)) return;
            this.value = value;
            OnValueChanged?.Invoke(this.value);
        }
    }
    protected override void OnReset()
    {
        ResetValue();
    }
    public virtual void ResetValue()
    {
        Value = initialValue;
    }
}


[CreateAssetMenu(fileName = "Int Variable", menuName = "Variable/Int")]
public class IntVariable : Variable<int>{}
