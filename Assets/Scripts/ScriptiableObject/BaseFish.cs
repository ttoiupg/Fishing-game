using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New fish", menuName = "Fish")]
public class BaseFish : ScriptableObject
{
    public Sprite Art;
    public float Weight;
    public int Age;
    public BaseRarity Rarity;
}
