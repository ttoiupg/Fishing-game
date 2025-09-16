using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CraftRequirement
{
    public GameItemSo item;
    public int amount;
}
[CreateAssetMenu(fileName = "CraftElement", menuName = "Game/Craft/Craft Element")]
public class CraftElement : ScriptableObject
{
    public GameItemSo craftItem;
    public int amount;
    public List<CraftRequirement> requirements;
}
