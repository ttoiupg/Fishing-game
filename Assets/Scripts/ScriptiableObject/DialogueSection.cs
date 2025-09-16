using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum DiablogueDisplayStyle
{
    Stellar,
    Tide,
    none,
}
[System.Serializable]
public enum DialogueSpeaker
{
    Right,
    Left,
    LeftSmall
}
[System.Serializable]
public struct DialogueOption
{
    public string name;
    public string text;
    public DialogueSection to; 
}
[System.Serializable]
public struct DialogueData
{
    public DialogueSpeaker speaker;
    public Sprite speakerSprite;
    public AudioClip Voice;
    public string speakerName;
    public string message;
    public float duration;
    public List<DialogueOption> options;
}


[CreateAssetMenu(fileName = "Dialogue", menuName = "Game/Dialogue/Dialogue")]
public class DialogueSection : ScriptableObject
{
    public string Speaker;
    public List<DialogueData> chats = new List<DialogueData>();
}