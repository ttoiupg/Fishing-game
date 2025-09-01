using System;
using UnityEngine;

public enum TimeSection
{
    morning,
    noon,
    afternoon,
    evening,
    night,
    lateNight,
}
[Serializable]
public enum DaysOfWeek
{
    Monday=1,
    Tuesday=2,
    Wednesday=3,
    Thursday=4,
    Friday=5,
    Saturday=6,
    Sunday=7,
}

public struct GameTime
{
    public int Hours;
    public int Minutes;
    public int totalMinutes;
    public TimeSection section;
    public DaysOfWeek daysOfWeek;

    public GameTime(int hours, int minutes, TimeSection section, DaysOfWeek daysOfWeek)
    {
        this.Hours = hours;
        this.Minutes = minutes;
        totalMinutes = hours * 60 + minutes;
        this.section = section;
        this.daysOfWeek = daysOfWeek;
    }
}
public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    public int Days;
    public DaysOfWeek DaysOfWeek;
    public int Hours;
    public int Minutes;
    [SerializeField]private int totalMinutes;
    public TimeSection currentSection;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Set(int days, int hours, int minutes, DaysOfWeek daysOfWeek)
    {
        this.Days = days;
        this.Hours = hours;
        this.Minutes = minutes;
        this.DaysOfWeek = daysOfWeek;
        totalMinutes = days * 1440 + hours * 60 + minutes;
        Tick(0);
    }
    public void Tick(int minutes)
    {
        totalMinutes += minutes;
        Hours = (totalMinutes / 60)%24;
        Days = totalMinutes / 1440;
        DaysOfWeek = (DaysOfWeek)(Enum.GetValues(typeof(DaysOfWeek)).GetValue((Days%7)));
        Minutes = totalMinutes % 60;
        switch (Hours)
        {
            case >=6 and <12:
                currentSection = TimeSection.morning;
                break;
            case >=12 and <13:
                currentSection = TimeSection.noon;
                break;
            case >=13 and <18:
                currentSection = TimeSection.afternoon;
                break;
            case >=18 and <21:
                currentSection = TimeSection.evening;
                break;
            case >=21 and <24:
                currentSection = TimeSection.night;
                break;
            case <6:
                currentSection = TimeSection.lateNight;
                break;
        }
    }
}