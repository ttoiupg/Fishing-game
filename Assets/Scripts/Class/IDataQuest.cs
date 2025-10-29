using System.Collections.Generic;

[System.Serializable]
public class IDataQuest
{
    public string id;
    public List<int> progress;

    public IDataQuest(Quest quest)
    {
        id = quest.questObject.ID;
        this.progress = new List<int>();
        for (int i = 0; i < quest.Steps.Count - 1; i++)
        {
            this.progress.Add(quest.Steps[i].GetProgress());
        }
    }
}