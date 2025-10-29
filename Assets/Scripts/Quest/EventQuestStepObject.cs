using UnityEngine;

[CreateAssetMenu(fileName = "EventQuestStepObject", menuName = "Quest/EventQuestStepObject")]
public class EventQuestStepObject :QuestStepDef
{
    public GameEvent questEvent;
    public override IQuestStep GetQuestStep() => new EventQuestStep(this);
    
}