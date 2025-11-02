using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest")]
public class QuestObject : ScriptableObject
{
    [SerializeField] private string id;
    public string ID => id;
    public string name;
    [TextArea]public string description;
    [SerializeReference]public List<QuestStepDef> steps = new();
    public QuestObject next;
}


public abstract class QuestStepDef :ScriptableObject
{
    public string text;
    public abstract IQuestStep GetQuestStep();
}