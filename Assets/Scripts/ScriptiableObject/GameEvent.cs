using System;
using UnityEngine;

[CreateAssetMenu(fileName ="Event",menuName = "Events/GameEvent")]
public class GameEvent : ScriptableObject
{
    public event Action Raised;
    public void Raise() => Raised?.Invoke();
}