using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Fishing rod", menuName = "Fishing rod")]
public class FishingRod : ScriptableObject
{
    public string description;
    public Sprite sprite;
    public BaseRarity rarity;
    public float damage;
    public float luck;
    public float resilience;
}