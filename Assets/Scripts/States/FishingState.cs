using UnityEngine;
using Halfmoon.StateMachine;
public class FishingState : BaseState
{
    public FishingState(Player player, Animator animator) : base(player, animator) { }

    public override void OnEnter()
    {
        Debug.Log("Entered fishing state");
        player.PlayerInputs.Fishing.CastFishingRod.performed += player.fishingController.HandleInput;
    }
    public override void OnExit() 
    {
        Debug.Log("Exit fishing state");
        player.PlayerInputs.Fishing.CastFishingRod.performed -= player.fishingController.HandleInput;
    }
}
