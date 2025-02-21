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

    }

    public void SaveData(ref GameData data)
    {

    }


}
[System.Serializable]
public class FishingRodData
{

}