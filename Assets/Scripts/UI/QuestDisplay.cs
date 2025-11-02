using TMPro;
using UnityEngine;

public class QuestDisplay : MonoBehaviour
{
    public TMP_Text questNameText;
    public TMP_Text questDescriptionText;
    [SerializeField] private QuestStepDisplay stepDisplayPrefab;
    private Quest quest;
    public void Setup(Quest quest)
    {
        this.quest = quest;
        questNameText.text = quest.questObject.name;
        questDescriptionText.text = quest.questObject.description;
        foreach (var step in quest.Steps)
        {
            var stepDisplay = Instantiate(stepDisplayPrefab, transform);
            stepDisplay.Setup(step);
        }
        quest.onComplete += () =>
        {
            Debug.Log("Finished quest: " + quest.questObject.name);
            QuestManager.Instance.RemoveQuest(quest);
            Destroy(gameObject,1.2f);
        };
    }
}
