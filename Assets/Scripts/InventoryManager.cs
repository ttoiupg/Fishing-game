using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IDataPersistence
{
    public static InventoryManager Instance;
    [Space] public List<FishingRod> fishingRods = new List<FishingRod>();
    [Space] public List<Fish> fishes = new List<Fish>();
    [Space] public List<GameItem> items = new List<GameItem>();
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void LoadData(GameData data)
    {
        //convert IDataFishingRod to fishingRod when load from GameData
        //clear list first to make sure its empty when adding
        fishingRods.Clear();
        //convert each IDataFishingRod and add into the list
        foreach (var dataRod in data.dataFishingRods)
        {
            fishingRods.Add(new FishingRod(dataRod));
        }
        //convert IDataFish to fish when load from GameData
        //clear list first to make sure its empty when adding
        fishes.Clear();
        //convert each IDataFish and add into the list
        foreach (var dataFish in data.dataFish)
        {
            fishes.Add(new Fish(dataFish));
        }
        items.Clear();
        foreach (var item in data.dataGameItem)
        {
            items.Add(new GameItem(item));
        }
    }

    public void SaveData(ref GameData data)
    {
        //convert fishingRod to IdataFishingRod for data save
        data.dataFishingRods.Clear();
        foreach (var fishingRod in fishingRods)
        {
            data.dataFishingRods.Add(new IDataFishingRod(fishingRod));
        }
        //convert fish to IDataFish for data save
        data.dataFish.Clear();
        foreach (var fish in fishes)
        {
            data.dataFish.Add(new IDataFish(fish));
        }
        data.dataGameItem.Clear();
        foreach (var gameItem in items)
        {
            data.dataGameItem.Add(new IDataGameItem(gameItem));
        }
    }
}