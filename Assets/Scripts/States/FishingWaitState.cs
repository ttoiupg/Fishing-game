using Halfmoon.StateMachine;
using UnityEngine;

public class FishingWaitState : BaseState
{
    public FishingWaitState(Player player, Animator animator) : base(player, animator) { }
    
    public override void OnEnter()
    {
        player.PlayerInputs.Fishing.CastFishingRod.performed += player.fishingController.HandleInput;
    }
    public override void OnExit() 
    {
        player.PlayerInputs.Fishing.CastFishingRod.performed -= player.fishingController.HandleInput;
    }
}
