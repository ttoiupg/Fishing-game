using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

[System.Serializable]
public class IDataFishingRod
{
    public string id;
    [FormerlySerializedAs("timeUsed")] public int fishCaught;
    public float durability;
    [FormerlySerializedAs("aquireDate")] public string acquireDate;
    public List<string> modifiers;
    public IDataFishingRod(FishingRod fishingRod)
    {
        id = fishingRod.fishingRodSO.id;
        fishCaught = fishingRod.fishCaught;
        durability = fishingRod.durability;
        acquireDate = fishingRod.acquireDate;
        modifiers = new List<string>();
    }
}

[System.Serializable]
public class IDataQuest
{
    public string id;
    public List<int> progress;
}