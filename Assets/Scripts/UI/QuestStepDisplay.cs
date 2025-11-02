using TMPro;
using UnityEngine;

public class QuestStepDisplay : MonoBehaviour
{
    public TMP_Text stepName;
    public TMP_Text stepGoal;
    public RectTransform progressBarFill;
    private IQuestStep step;

    public void Setup(IQuestStep step)
    {
        this.step = step;
        stepName.text = step.text;
        stepGoal.text = $"{step.GetProgress()}/{step.goal}";
        progressBarFill.localScale = new Vector3(step.GetProgress() / step.goal, 1, 1);
        step.onUpdate += updateDisplay;
    }

    private void updateDisplay(IQuestStep step, float progress)
    {
        Debug.Log($"update step ui {progress}");
        stepGoal.text = $"{step.GetProgress()}/{step.goal}";
        progressBarFill.localScale = new Vector3(progress / step.goal, 1, 1);
    }
}