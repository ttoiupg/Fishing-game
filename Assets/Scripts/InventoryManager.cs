using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IDataPersistence
{
    public static InventoryManager Instance;
    [Space]
    public List<FishingRod> fishingRods = new List<FishingRod>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void LoadData(GameData data)
    {
        fishingRods.Clear();
        foreach(IDataFishingRod dataRod in data.dataFishingRods)
        {
            fishingRods.Add(new FishingRod(dataRod));
        }
    }

    public void SaveData(ref GameData data)
    {
        data.dataFishingRods.Clear();
        foreach (FishingRod fishingRod in fishingRods)
        {
            data.dataFishingRods.Add(new IDataFishingRod(fishingRod));
        }
    }
}