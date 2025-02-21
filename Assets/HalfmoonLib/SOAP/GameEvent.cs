using System.Collections.Generic;
using UnityEngine;

public class GameEvent<T> : ScriptableObject
{
    readonly List<IGameEventListener<T>> listeners = new();

    public void Fire(T data)
    {
        for (var i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventFired(data);
        }
    }

    public void RegisterListener(IGameEventListener<T> listener) => listeners.Add(listener);
    public void DeregisterListener(IGameEventListener<T> listener) => listeners.Remove(listener);
}

// this is for no param event
[CreateAssetMenu(menuName = "Events/Game Event")]
public class GameEvent : GameEvent<Unit>
{
    public void Fire() => Fire(Unit.Default);
}

public struct Unit
{
    public static Unit Default => default;
}
