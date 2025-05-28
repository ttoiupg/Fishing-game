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
        fishingRod.modifiers.ForEach(modifier => modifiers.Add(modifier.id));
    }
}