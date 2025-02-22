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
}