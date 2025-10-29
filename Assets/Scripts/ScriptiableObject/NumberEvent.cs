using System;
using UnityEngine;

[CreateAssetMenu(fileName ="NumberEvent",menuName = "Events/NumberEvent")]
public class NumberEvent : ScriptableObject
{
    public event Action<int> Raised;
    public void Raise(int value) => Raised?.Invoke(value);
}