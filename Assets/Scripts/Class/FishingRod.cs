using UnityEngine;

[System.Serializable]
public class FishingRod
{
    public FishingRodSO fishingRodSO;
    public int timeUsed;
    public float durability;
    public string aquireDate;

    public FishingRod(FishingRodSO fishingRodSO, int timeUsed, float durability, string aquireDate)
    {
        this.fishingRodSO = fishingRodSO;
        this.timeUsed = timeUsed;
        this.durability = durability;
        this.aquireDate = aquireDate;
    }
    public FishingRod(IDataFishingRod fishingRod)
    {
        this.fishingRodSO = DataPersistenceManager.Instance.gameFishingRods[fishingRod.id];
        this.timeUsed = fishingRod.timeUsed;
        this.durability = fishingRod.durability;
        this.aquireDate = fishingRod.aquireDate;
    }
}