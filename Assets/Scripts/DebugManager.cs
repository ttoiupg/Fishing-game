using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugManager : PlayerSystem
{

    public TextMeshProUGUI CatchedFishText;
    public TextMeshProUGUI CatchedMutationText;
    public TextMeshProUGUI CurrentZoneText;
    public TextMeshProUGUI IsFishingText;
    public TextMeshProUGUI RarityText;

    private void Update()
    {
        if (player.ID.currentZone != null)
        {
            CurrentZoneText.text = $"In {player.ID.currentZone.name}";
        }
        else
        {
            CurrentZoneText.text = "Not in fishing zone";
        }
    }
    private void ShowCatchedFishStats(Fish fish)
    {
        CatchedFishText.text = $"Catched : {fish.fishType.name}";
        RarityText.text = $"Rarity : {fish.fishType.Rarity.name}";
        CatchedMutationText.text = $"With mutation : {fish.mutation.name}";
    }
    private void UpdateFishingState()
    {
        if (player.ID.isFishing)
        {
            IsFishingText.text = "Is fishing";
        }
        else
        {
            IsFishingText.text = "Not fishing";
            CatchedFishText.text = "Catched : ";
            RarityText.text = "Rarity : ";
            CatchedMutationText.text = "With mutation : ";

        }
    }
    private void OnEnable()
    {
        UpdateFishingState();
        player.ID.playerEvents.OnFishCatched += ShowCatchedFishStats;
        player.ID.playerEvents.OnEnterFishingState += UpdateFishingState;
        player.ID.playerEvents.OnExitFishingState += UpdateFishingState;
    }

    private void OnDisable()
    {
        player.ID.playerEvents.OnFishCatched -= ShowCatchedFishStats;
        player.ID.playerEvents.OnEnterFishingState -= UpdateFishingState;
        player.ID.playerEvents.OnExitFishingState -= UpdateFishingState;
        CatchedFishText.text = "";
        CatchedFishText.text = "";
        CatchedMutationText.text = "";
        CurrentZoneText.text = "";
        IsFishingText.text = "";
        RarityText.text = "";
    }
}
