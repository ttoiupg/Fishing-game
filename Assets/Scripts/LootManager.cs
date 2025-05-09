using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    /// <summary>
    /// add fishing rod to player's inventory(inventory manager)
    /// base on the param
    /// </summary>
    /// <param name="fishingRod"></param>
    public void AddFishingRod(FishingRod fishingRod)
    {
        InventoryManager.Instance.fishingRods.Add(fishingRod);
    }
    /// <summary>
    /// add fish to player's inventory(inventory manager)
    /// base on the param
    /// </summary>
    /// <param name="fish"></param>
    public void AddFish(Fish fish)
    {
        InventoryManager.Instance.fishes.Add(fish);
    }
}