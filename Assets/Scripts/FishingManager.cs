using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public static FishingManager instance;

    public Player player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public void CatchedFish(Fish fish)
    {
        //check if this is the first time player catches this fish
        if (!player.UnlockedFishes.Contains(fish.fishType))
        {
            player.UnlockedFishes.Add(fish.fishType);
        }
        player.experience += fish.fishType.Experience;
    }
}
