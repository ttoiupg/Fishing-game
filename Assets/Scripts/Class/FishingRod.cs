using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class FishingRod
{
    public FishingRodSO fishingRodSO;
    [FormerlySerializedAs("timeUsed")] public int fishCaught;
    public float durability;
    public string aquireDate;

    public FishingRod(FishingRodSO fishingRodSO, int fishCaught, float durability, string aquireDate)
    {
        this.fishingRodSO = fishingRodSO;
        this.fishCaught = fishCaught;
        this.durability = durability;
        this.aquireDate = aquireDate;
    }
    public FishingRod(IDataFishingRod fishingRod)
    {
        this.fishingRodSO = DataPersistenceManager.Instance.gameFishingRods[fishingRod.id];
        this.fishCaught = fishingRod.fishCaught;
        this.durability = fishingRod.durability;
        this.aquireDate = fishingRod.aquireDate;
    }
}