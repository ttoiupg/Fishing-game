using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;

[System.Serializable]
public class IDataQuest
{
    public string id;
    public List<int> progress;

    public IDataQuest(Quest quest)
    {
        id = quest.questObject.ID;
        this.progress = new List<int>();
        for (int i = 0; i < quest.Steps.Count; i++)
        {
            this.progress.Add(quest.Steps[i].GetProgress());
        }
    }
}

[System.Serializable]
public class IDataStoryIntValue
{
    public string id;
    public int value;

    public IDataStoryIntValue(IntVariable intVariable)
    {
        id = intVariable.id;
        value = intVariable.Value;
    }
}
[System.Serializable]
public class IDataStoryBoolValue
{
    public string id;
    public bool value;

    public IDataStoryBoolValue(BoolVariable intVariable)
    {
        id = intVariable.id;
        value = intVariable.Value;
    }
}