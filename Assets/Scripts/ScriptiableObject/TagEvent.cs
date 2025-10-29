using System;
using UnityEngine;

[CreateAssetMenu(fileName ="TagEvent",menuName = "Events/TagEvent")]
public class TagEvent : ScriptableObject
{
    public event Action<EventTag> Raised;
    public EventTag RaisedTag;
    public void Raise() => Raised?.Invoke(RaisedTag);
}