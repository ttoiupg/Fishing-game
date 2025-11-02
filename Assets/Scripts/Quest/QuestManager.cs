using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class QuestManager : MonoBehaviour,IDataPersistence
{
    public static QuestManager Instance { get; private set; }
    public List<Quest> quests;
    public Action<Quest> onQuestAdded;
    [SerializeField] private int currentIndex = 0;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        quests = new List<Quest>();
    }
    public static QuestObject LoadCharacter(string id)
    {
        return DataPersistenceManager.Instance.Quests[id];
    }
    public void AddQuest(QuestObject questObject, List<int> progress)
    {
        var quest = new Quest(questObject, progress,currentIndex);
        quest.Start();
        quests.Add(quest);
        onQuestAdded?.Invoke(quest);
        currentIndex++;
    }
    public void AddQuestEmpty(QuestObject questObject)
    {
        var quest = new Quest(questObject,currentIndex);
        quest.Start();
        quests.Add(quest);
        onQuestAdded?.Invoke(quest);
        currentIndex++;
    }
    public void AddQuestEmpty(QuestObject questObject, int index)
    {
        var quest = new Quest(questObject, index);
        quest.Start();
        quests.Add(quest);
        onQuestAdded?.Invoke(quest);
    }
    public void RemoveQuest(Quest quest)
    {
        quest.Stop();
        quests.Remove(quest);
    }
    public void LoadData(GameData data)
    {
        var objs = data.playerData.questList.Select(g => LoadCharacter(g.id)).ToList();
        var progress = data.playerData.questList.Select(g => g.progress).ToList();
        for (int i = 0; i < objs.Count; i++)
        {
            AddQuest(objs[i], progress[i]);
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
    float goal { get; }
    Action<IQuestStep,float> onUpdate { get; set; }
    void Bind();
    void Unbind();
    int GetProgress();
    void SetProgress(int value);
}
[System.Serializable]
public sealed class EventQuestStep : IQuestStep
{
    readonly EventQuestStepObject so;
    private bool done;
    public string text => so.text;
    public bool isCompleted => done;
    public float progress01 => done?1f:0f;
    public float goal => 1f;

    public Action<IQuestStep,float> onUpdate { get; set; }

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
        onUpdate?.Invoke(this,GetProgress());
    }
    private void OnTriggerd(){
        done = true;
        Debug.Log("EventQuestStep triggered: " + text);
        onUpdate?.Invoke(this, GetProgress());
    }

    public int GetProgress()
    {
        return done?1:0;
    }
}
[System.Serializable]
public sealed class ValueQuestStep : IQuestStep
{
    readonly ValueQuestStepObject so;
    private int progress;
    public string text => so.text;
    public bool isCompleted => progress >= so.goal;
    public float progress01 => Mathf.Clamp01((float)progress / so.goal);
    public float goal => so.goal;
    public Action<IQuestStep, float> onUpdate { get; set; }

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
        onUpdate?.Invoke(this,GetProgress());
    }
    private void OnTriggerd(int value)
    {
        progress += value;
        if (progress >= so.goal) progress = so.goal;
        onUpdate?.Invoke(this, GetProgress());
        Debug.Log("ValueQuestStep triggered: " + text);
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
    public Action onComplete;
    public readonly int index;
    public Quest(QuestObject questObject, int index)
    {
        this.index = index;
        this.questObject = questObject;
        steps = questObject.steps.Select(s => s.GetQuestStep()).ToList();
        this.index = index;
    }

    public Quest(QuestObject questObject, List<int> preset,int index)
    {
        this.index = index;
        this.questObject = questObject;
        steps = questObject.steps.Select(s => s.GetQuestStep()).ToList();
        for (int i = 0; i < steps.Count; i++)
        {
            if (i < preset.Count)
                steps[i].SetProgress(preset[i]);
            else
            {
                steps[i].SetProgress(0);
            }
            steps[i].onUpdate += CheckCompletion;
        }
    }

    private void CheckCompletion(IQuestStep step, float progress)
    {
        Debug.Log($"Checking quest completion for {step.text} {progress}");
        if (IsCompleted)
        {
            Debug.Log("Quest completed: " + questObject.name);
            onComplete?.Invoke();
            if (questObject.next != null)
            {
                QuestManager.Instance.AddQuestEmpty(questObject.next,this.index);
            }
        }
    }
    public bool IsCompleted => steps.All(s => s.isCompleted);
    public IReadOnlyList<IQuestStep> Steps => steps;

    public void Start()
    {
        foreach (var s in steps) 
        {
            s.Bind();
        }
    }

    public void Stop()
    {
        foreach (var s in steps) { s.Unbind(); }
    }
    
}