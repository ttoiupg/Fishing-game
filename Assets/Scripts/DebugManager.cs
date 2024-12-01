using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DebugManager : PlayerSystem
{
    public UIDocument DebugUI;
    public VisualElement Container;
    public Label CatchedFishText;
    public Label CatchedMutationText;
    public Label CurrentZoneText;
    public Label IsFishingText;
    public Label RarityText;

    private void Start()
    {
        Container = DebugUI.rootVisualElement;
        CatchedFishText = Container.Q("FishText") as Label;
        CatchedMutationText = Container.Q("MutationText") as Label;
        RarityText = Container.Q("RarityText") as Label;
        CurrentZoneText = Container.Q("ZoneText") as Label;
        IsFishingText = Container.Q("StateText") as Label;
        UpdateFishingState();
    }
    private void Update()
    {
        if (player.ID.currentZone != null)
        {
            CurrentZoneText.text = $"Current zone:{player.ID.currentZone.name}";
        }
        else
        {
            CurrentZoneText.text = "Current zone:none";
        }
    }
    private void ShowCatchedFishStats(Fish fish)
    {
        CatchedFishText.text = $"Catched fish:{fish.fishType.name}";
        RarityText.text = $"Fish rarity:{fish.fishType.Rarity.name}";
        CatchedMutationText.text = $"Fish mutation:{fish.mutation.name}";
    }
    private void UpdateFishingState()
    {
        if (player.ID.isFishing)
        {
            IsFishingText.text = "Current state:Is fishing";
        }
        else
        {
            IsFishingText.text = "Current state:Not fishing";
            CatchedFishText.text = "Catched fish:";
            RarityText.text = "Fish rarity:";
            CatchedMutationText.text = "Fish mutation:";
        }
    }
    private void OnEnable()
    {
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
