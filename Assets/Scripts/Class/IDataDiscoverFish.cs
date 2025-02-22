using UnityEngine;
using System;
[System.Serializable]
public class IDataDiscoverFish
{
    public string id;
    public string discoverDate;
    public int timeCatched;
    public IDataDiscoverFish(DiscoveredFish fish)
    {
        id = fish.baseFish.id;
        discoverDate = fish.discoverDate;
        timeCatched = fish.timeCatched;
    }
}
public class IDataFishingRod
{
    public string id;
    public int timeUsed;
    public float durability;
    public string aquireDate;
}