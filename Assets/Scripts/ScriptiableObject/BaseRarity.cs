using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rarity", menuName = "Rarity")]
public class BaseRarity : ScriptableObject
{
    public string Description;
    public Sprite Color;
    public float OneIn;
}
