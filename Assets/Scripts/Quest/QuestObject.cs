using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest")]
public class QuestObject : ScriptableObject
{
    [SerializeField] private string id;
    public string ID => id;
    public string name;
    public string description;
    public List<QuestItem> items;
}

public interface QuestItem
{
    int OnEvent(int value);
}

public abstract class BaseQuestObject :ScriptableObject, QuestItem
{
    string text;

    public virtual int OnEvent(int value)
    {
        return 1;
    }
}

[CreateAssetMenu(fileName = "EventQuest", menuName = "Quest/EventQuest")]
public class EventQuestObject :BaseQuestObject
{
    public GameEvent Event;

    public override int OnEvent(int value)
    {
        return 1;
    }
    
}
[CreateAssetMenu(fileName = "ValueGoalQuest", menuName = "Quest/ValueGoalQuest")]
public class ValueGoalQuestObject :BaseQuestObject
{
    public NumberEvent valueEvent;
    public int goal;
    public override int OnEvent(int value)
    {
        return value;
    }
}