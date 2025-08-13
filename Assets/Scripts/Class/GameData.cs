using System;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Serialization;

[System.Serializable]
public class GameData
{
    public PlayerData playerData;
    public FishingRodData fishingRodData;
    public ItemData itemData;

    public GameData()
    {
        this.playerData = new PlayerData();
        this.fishingRodData = new FishingRodData();
        this.itemData = new ItemData();
    }
}

[System.Serializable]
public class PlayerData
{
    public int level;
    public int gold;
    public float experience;
    public int equipedFishingRod;
    public List<IDataDiscoverFish> discoverFishList = new List<IDataDiscoverFish>();
    public List<String> modifiers = new List<String>();

    public PlayerData()
    {
        this.gold = 0;
        this.level = 1;
        this.experience = 0;
        equipedFishingRod = 0;
        discoverFishList = new List<IDataDiscoverFish>();
        modifiers = new List<String>();
    }
}
[System.Serializable]
public class FishingRodData
{
    public List<IDataFishingRod> fishingRods = new List<IDataFishingRod>();

    public FishingRodData()
    {
        fishingRods = new List<IDataFishingRod>();
    }
}

[System.Serializable]
public class ItemData
{
    public List<IDataFish> fishes = new List<IDataFish>();
    public List<IDataGameItem> gameItems = new List<IDataGameItem>();

    public ItemData()
    {
        fishes = new List<IDataFish>();
        gameItems = new List<IDataGameItem>();
    }
}
