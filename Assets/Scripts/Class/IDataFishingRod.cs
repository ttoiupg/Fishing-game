[System.Serializable]
public class IDataFishingRod
{
    public string id;
    public int timeUsed;
    public float durability;
    public string aquireDate;
    public IDataFishingRod(FishingRod fishingRod)
    {
        id = fishingRod.fishingRodSO.id;
        timeUsed = fishingRod.timeUsed;
        durability = fishingRod.durability;
        aquireDate = fishingRod.aquireDate;
    }
}