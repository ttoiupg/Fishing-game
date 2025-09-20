using System.Collections.Generic;
using UnityEngine;

public enum CraftRequirementType
{
    Item,
    Fish
}
[System.Serializable]
public struct CraftRequirement
{
    public CraftRequirementType type;
    public GameItemSo item;
    public BaseFish fish;
    public int amount;
}
[CreateAssetMenu(fileName = "CraftElement", menuName = "Game/Craft/Craft Element")]
public class CraftElement : ScriptableObject
{
    public GameItemSo craftItem;
    public int amount;
    public List<CraftRequirement> requirements;
}
