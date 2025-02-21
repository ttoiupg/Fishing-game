using UnityEngine;
using UnityEngine.Events;

public interface IGameEventListener<T>
{
    void OnEventFired(T data);
}

public class GameEventListener<T> : MonoBehaviour, IGameEventListener<T>
{
    [SerializeField] GameEvent<T> gameEvent;
    [SerializeField] UnityEvent<T> response;

    void OnEnable() => gameEvent.RegisterListener(this);
    void OnDisable() => gameEvent.DeregisterListener(this);

    public void OnEventFired(T data) => response.Invoke(data);
}

public class GameEventListener : GameEventListener<Unit> { }