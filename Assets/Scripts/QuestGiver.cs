using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] private QuestObject quest;

    public void AddQuestToPlayer()
    {
        QuestManager.Instance.AddQuestEmpty(quest);
    }
}