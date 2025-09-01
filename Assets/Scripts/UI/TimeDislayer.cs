using TMPro;
using UnityEngine;

public class TimeDislayer : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI timeSectionText;
    public TextMeshProUGUI dayCount;

    private void Update()
    {
        var hour = TimeManager.Instance.Hours;
        var minute = TimeManager.Instance.Minutes;
        var section = TimeManager.Instance.currentSection.ToString();
        timeText.text = string.Format("{0:00}:{1:00}", hour, minute);
        timeSectionText.text = section;
        dayCount.text = $"Day: {TimeManager.Instance.Days+1} ({TimeManager.Instance.DaysOfWeek.ToString()})";
    }
}