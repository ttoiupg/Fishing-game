using UnityEngine;

[CreateAssetMenu(fileName = "ValueQuestStepObject", menuName = "Quest/ValueQuestStepObject")]
public class ValueQuestStepObject :QuestStepDef
{
    public NumberEvent questEvent;
    public int goal;
    public override IQuestStep GetQuestStep() => new ValueQuestStep(this);
}