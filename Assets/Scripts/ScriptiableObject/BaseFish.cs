using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New fish", menuName = "Fish")]
public class BaseFish : ScriptableObject, IComparable<BaseFish>
{
    public Sprite Art;
    public float MinWeight;
    public float MaxWeight;
    public string FavoriteFood;
    public BaseRarity Rarity;
    public float MinFishBarChangeTime;
    public float MaxFishBarChangeTime;
    public float MaxFishBarChangeDistance;
    public float Experience;

    [Header("Display")]
    public Sprite Ring;
    public Sprite Card;

    public int CompareTo(BaseFish compareFish)
    {
        if (compareFish == null)
        {
            return 1;
        }
        else
        {
            float rarityA = 1f / this.Rarity.OneIn;
            float rarityB = 1f / compareFish.Rarity.OneIn;
            return rarityA.CompareTo(rarityB);
        }
    }
}
