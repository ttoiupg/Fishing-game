using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New fish", menuName = "Fish")]
public class BaseFish : ScriptableObject
{
    public string Name;
    public Sprite Art;
    public float Weight;
    public int Age;
    public string Rarity;
}
