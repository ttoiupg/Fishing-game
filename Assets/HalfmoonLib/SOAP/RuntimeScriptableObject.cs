using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeScriptableObject : ScriptableObject
{
    static readonly List<RuntimeScriptableObject> Instances = new();
    void OnEnable() => Instances.Add(this);
    void OnDisable() => Instances.Remove(this);

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
