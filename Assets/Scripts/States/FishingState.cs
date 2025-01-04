using UnityEngine;
using Halfmoon.StateMachine;
public class FishingState : BaseState
{
    public FishingState(Player player, Animator animator) : base(player, animator) { }

    public override void OnEnter()
    {
        Debug.Log("enter fishing state");
        player.playerInputs.Fishing.CastFishingRod.performed += player.fishingController.CastOrRetract;
    }
    public override void OnExit() 
    {
        Debug.Log("exit fishing state");
        player.playerInputs.Fishing.CastFishingRod.performed -= player.fishingController.CastOrRetract;
    }
}
