using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rarity", menuName = "Game/Rarity")]
public class BaseRarity : ScriptableObject
{
    public string id;
    public float OneIn;
    public Color InventoryColor;
    public Sprite TagSprite;
}