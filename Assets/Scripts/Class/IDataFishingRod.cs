using UnityEngine.Serialization;

[System.Serializable]
public class IDataFishingRod
{
    public string id;
    [FormerlySerializedAs("timeUsed")] public int fishCaught;
    public float durability;
    public string aquireDate;
    public IDataFishingRod(FishingRod fishingRod)
    {
        id = fishingRod.fishingRodSO.id;
        fishCaught = fishingRod.fishCaught;
        durability = fishingRod.durability;
        aquireDate = fishingRod.aquireDate;
    }
}