using UnityEngine;
using Halfmoon.StateMachine;

public class FishingBoostState : BaseState
{
    public FishingBoostState(Player player, Animator animator) : base(player, animator) { }

    public override void OnEnter()
    {
        player.PlayerInputs.Fishing.CastFishingRod.performed += player.fishingController.LandPointer;
        player.fishingController.EnterBoostState();
    }
    public override void OnExit()
    {
        player.PlayerInputs.Fishing.CastFishingRod.performed -= player.fishingController.LandPointer;
    }
}
