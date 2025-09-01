using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TimeManager))]
public class TimeManagerEditor : Editor
{
    public int day;
    public DaysOfWeek daysOfWeek;
    public int setHours;
    public int setMinutes;
    public override void OnInspectorGUI()
    {
        TimeManager timeManager = (TimeManager)target;
        EditorGUILayout.LabelField("Time");
        base.OnInspectorGUI();
        EditorGUILayout.Space(10);
        if (GUILayout.Button("15 minutes"))
        {
            timeManager.Tick(15);
        }
        if (GUILayout.Button("An Hour"))
        {
            timeManager.Tick(60);
        }
        if (GUILayout.Button("A day"))
        {
            timeManager.Tick(1440);
        }
        EditorGUILayout.Space(30);
        day = EditorGUILayout.IntField("Day",day);
        daysOfWeek = (DaysOfWeek)EditorGUILayout.EnumPopup("Days of week",daysOfWeek);
        EditorGUILayout.LabelField("Hour");
        setHours = EditorGUILayout.IntSlider(setHours,0,24);
        EditorGUILayout.LabelField("Minutes");
        setMinutes = EditorGUILayout.IntSlider(setMinutes,0,60);
        if (GUILayout.Button("Set"))
        {
            timeManager.Set(day,setHours,setMinutes,daysOfWeek);
        }
    }
}