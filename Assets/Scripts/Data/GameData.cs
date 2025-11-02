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
    public GlobalSettings globalSettings;

    public GameData()
    {
        this.globalSettings = new GlobalSettings();
        this.playerData = new PlayerData();
        this.fishingRodData = new FishingRodData();
        this.itemData = new ItemData();
    }
}

[System.Serializable]
public class PlayerData
{
    public string name;
    public int level;
    public int gold;
    public float experience;
    public int equipedFishingRod;
    public List<IDataDiscoverFish> discoverFishList = new List<IDataDiscoverFish>();
    public List<String> modifiers = new List<String>();
    public Vector3 position;
    public string Scene;
    public List<IDataQuest>  questList = new List<IDataQuest>();
    public List<IDataStoryIntValue> storyIntValues = new List<IDataStoryIntValue>();
    public List<IDataStoryBoolValue> storyBoolValues = new List<IDataStoryBoolValue>();

    public PlayerData()
    {
        this.gold = 0;
        this.level = 1;
        this.experience = 0;
        equipedFishingRod = 0;
        discoverFishList = new List<IDataDiscoverFish>();
        modifiers = new List<String>();
        position = new Vector3(-7.14900017f,4.46799994f,0);
        questList = new List<IDataQuest>();
        storyIntValues = new List<IDataStoryIntValue>();
        storyBoolValues = new List<IDataStoryBoolValue>();
        Scene = "CenterTown";
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
public enum AntiAlias
{
    None = 0,
    FXAA = 1,
    SMAA = 2,
    TAA = 3
}
[System.Serializable]
public class GlobalSettings
{
    public AntiAlias antiAlias;
    public Vector2Int screenSize;
    public float masterVolume;
    public int Fullscreen;
    public int framerate;

    public GlobalSettings()
    {
        this.antiAlias = AntiAlias.FXAA;
        this.screenSize = new Vector2Int(1920, 1080);
        this.masterVolume = 0.5f;
        this.Fullscreen = 0;
        this.framerate = -1;
    }
}
