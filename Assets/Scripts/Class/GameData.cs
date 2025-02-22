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

    public Dictionary<string,DiscoveredFish> discoveredFish;
    public List<IDataDiscoverFish> dataDiscoverFishList = new List<IDataDiscoverFish>();

    public List<FishingRod> ownedFishingRods;

    public GameData()
    {
        FishingRod starterRod = new FishingRod();
        this.level = 1;
        this.experience = 0;
        discoveredFish = new Dictionary<string, DiscoveredFish>();
        ownedFishingRods = new List<FishingRod>();
    }
    public void Init()
    {
        foreach(IDataDiscoverFish dis in dataDiscoverFishList)
        {
            BaseFish bf;
            DataPersistenceManager.Instance.gameFish.TryGetValue(dis.id,out bf);
            discoveredFish.Add(dis.id, new DiscoveredFish(bf, dis.discoverDate));
        }
    }
    public void Prepare()
    {
        foreach(DiscoveredFish fish in discoveredFish.Values)
        {
            dataDiscoverFishList.Add(new IDataDiscoverFish(fish));
        }
    }
}
