using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int level;
    public float experience;
    public List<DiscoveredFish> discoveredFish;
    public GameData()
    {
        this.level = 1;
        this.experience = 0;
        discoveredFish = new List<DiscoveredFish>();
    }
}
