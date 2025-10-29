using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class QuestManager : MonoBehaviour,IDataPersistence
{
    public List<Quest> quests;
    public static QuestObject LoadCharacter(string id)
    {
        return DataPersistenceManager.Instance.Quests[id];
    }
    public void LoadData(GameData data)
    {
        var objs = data.playerData.questList.Select(g => LoadCharacter(g.id)).ToList();
        var progress = data.playerData.questList.Select(g => g.progress).ToList();
        for (int i = 0; i < objs.Count; i++)
        {
            var quest = new Quest(objs[i],progress[i]);
            quest.Start();
            quests.Add(quest);
        }
    }

    public void SaveData(ref GameData data)
    {
        var IDataQuests = quests.Select(q => new IDataQuest(q)).ToList();
        data.playerData.questList = IDataQuests;
    }
}

public interface IQuestStep
{
    string text { get; }
    bool isCompleted { get; }
    float progress01 { get; }
    void Bind();
    void Unbind();
    int GetProgress();
    void SetProgress(int value);
}

public sealed class EventQuestStep : IQuestStep
{
    readonly EventQuestStepObject so;
    private bool done;
    public string text => so.text;
    public bool isCompleted => done;
    public float progress01 => done?1f:0f;
    
    public EventQuestStep(EventQuestStepObject so) => this.so = so;
    
    public void Bind()
    {
        so.questEvent.Raised += OnTriggerd;
    }

    public void Unbind()
    {
        so.questEvent.Raised -= OnTriggerd;
    }
    public void SetProgress(int value)
    {
        done = value == 1;
    }
    private void OnTriggerd(){done = true;}

    public int GetProgress()
    {
        return done?1:0;
    }
}

public sealed class ValueQuestStep : IQuestStep
{
    readonly ValueQuestStepObject so;
    private int progress;
    public string text => so.text;
    public bool isCompleted => progress >= so.goal;
    public float progress01 => Mathf.Clamp01((float)progress / so.goal);
    
    public ValueQuestStep(ValueQuestStepObject so) => this.so = so;
    
    public void Bind()
    {
        so.questEvent.Raised += OnTriggerd;
    }

    public void Unbind()
    {
        so.questEvent.Raised -= OnTriggerd;
    }

    public void SetProgress(int value)
    {
        progress = value;
        if (progress >= so.goal) progress = so.goal;
    }
    private void OnTriggerd(int value)
    {
        progress += value;
        if (progress >= so.goal) progress = so.goal;
    }

    public int GetProgress()
    {
        return progress;
    }
}

[System.Serializable]
public sealed class Quest
{
    public readonly QuestObject questObject;
    readonly List<IQuestStep> steps;

    public Quest(QuestObject questObject)
    {
        this.questObject = questObject;
        steps = questObject.items.Select(s => s.GetQuestStep()).ToList();
    }

    public Quest(QuestObject questObject, List<int> preset)
    {
        this.questObject = questObject;
        steps = questObject.items.Select(s => s.GetQuestStep()).ToList();
        for (int i = 0; i < steps.Count; i++)
        {
            steps[i].SetProgress(preset[i]);
        }
    }
    public bool IsCompleted => steps.All(s => s.isCompleted);
    public IReadOnlyList<IQuestStep> Steps => steps;

    public void Start()
    {
        foreach (var s in steps) { s.Bind(); }
    }

    public void Stop()
    {
        foreach (var s in steps) { s.Unbind(); }
    }
    
}