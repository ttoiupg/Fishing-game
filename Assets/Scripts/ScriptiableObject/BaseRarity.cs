using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rarity", menuName = "Rarity")]
public class Rarity : ScriptableObject
{
    public string Description;
    public Sprite Color;
    public int Weight;
    public int OneIn;
}
