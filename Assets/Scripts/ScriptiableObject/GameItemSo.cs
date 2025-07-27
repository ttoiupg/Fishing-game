using UnityEngine;

public enum ItemType
{
    Bait,
    StoryItem,
    Element,
}

[CreateAssetMenu(fileName = "New GameItem", menuName = "Game/GameItem")]
public class GameItemSo : ScriptableObject
{
    public string id;
    public string name;
    public string description;
    public BaseRarity rarity;
    public ItemType itemType;
    [Header("Display")]
    public Sprite Ring;
    public Sprite icon;
}
