using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryManager : MonoBehaviour, IDataPersistence
{
    public GameObject newGameFrame;
    public TMP_InputField nameInputField;
    public DialogueSection introDialogue;

    [SerializeField] private List<IntVariable> intVariables;
    [SerializeField] private List<BoolVariable> boolVariables;

    private Dictionary<string, IntVariable> intVariableDict;
    private Dictionary<string, BoolVariable> boolVariableDict;

    private void StartNewGameFrame()
    {
        newGameFrame.SetActive(true);
    }
    public void NameEntered()
    {
        newGameFrame.SetActive(false);
        boolVariableDict["Bool_IsNewGame"].Value = false;
        GameManager.Instance.player.playerName.Value = nameInputField.text;
        GameManager.Instance.player.isActive = true;
        DialogueManager.Instance.StartDialogue(introDialogue,GameManager.Instance.player.transform);
    }

    private void Start()
    {
        intVariableDict = new Dictionary<string, IntVariable>();
        foreach (var intVar in intVariables)
        {
            intVariableDict[intVar.id] = intVar;
        }
        boolVariableDict = new Dictionary<string, BoolVariable>();
        foreach (var boolVar in boolVariables)
        {
            boolVariableDict[boolVar.id] = boolVar;
        }
    }
    public void LoadData(GameData data)
    {
        if (data.playerData.storyIntValues == null)
        {
            data.playerData.storyIntValues = new List<IDataStoryIntValue>();
        }
        if (data.playerData.storyBoolValues == null)
        {
            data.playerData.storyBoolValues = new List<IDataStoryBoolValue>();
        }
        foreach (var intVar in data.playerData.storyIntValues)
        {
            intVariableDict[intVar.id].Value = intVar.value;
        }
        foreach (var boolVar in data.playerData.storyBoolValues)
        {
            boolVariableDict[boolVar.id].Value = boolVar.value;
        }
        if (boolVariableDict["Bool_IsNewGame"].Value == true)
        {
            StartNewGameFrame();
            GameManager.Instance.player.isActive = false;
        }
    }
    public void SaveData(ref GameData data)
    {
        data.playerData.storyBoolValues.Clear();
        data.playerData.storyIntValues.Clear();
        foreach (var intVar in intVariables)
        {
            data.playerData.storyIntValues.Add(new IDataStoryIntValue(intVar));
        }
        foreach (var boolVar in boolVariables)
        {
            data.playerData.storyBoolValues.Add(new IDataStoryBoolValue(boolVar));
        }
    }
}