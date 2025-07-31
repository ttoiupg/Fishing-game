using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Fishing rod", menuName = "Game/Fishing rod")]
public class FishingRodSO : ScriptableObject
{
    public string id;
    public string description;
    public Sprite spriteDisplay;
    public Sprite spriteWorldRod;
    public Sprite spriteWorldReel;
    public BaseRarity rarity;
    public float damage;
    public float accuracy;
    public float critChance;
    public float critMultiplier;
    public float luck;
    public float resilience;
    public float durability;
}