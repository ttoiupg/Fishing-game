using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

[System.Serializable]
public class GameData
{
    public int level;
    public float experience;
    public int equipedFishingRod;
    public List<IDataDiscoverFish> dataDiscoverFishList = new List<IDataDiscoverFish>();
    public List<IDataFishingRod> dataFishingRods = new List<IDataFishingRod>();
    public List<IDataFish> dataFish = new List<IDataFish>();

    public GameData()
    {
        this.level = 1;
        this.experience = 0;
        equipedFishingRod = 0;
        dataDiscoverFishList = new List<IDataDiscoverFish>();
        dataFishingRods = new List<IDataFishingRod>();
        dataFish = new List<IDataFish>();
    }
}
