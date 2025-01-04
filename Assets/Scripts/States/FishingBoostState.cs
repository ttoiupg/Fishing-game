using UnityEngine;
using Halfmoon.StateMachine;

public class FishingBoostState : BaseState
{
    public FishingBoostState(Player player, Animator animator) : base(player, animator) { }

    public override void OnEnter()
    {
        Debug.Log("enter boost state");
        player.playerInputs.Fishing.CastFishingRod.performed += player.fishingController.LandPointer;
        player.fishingController.EnterBoostState();
    }
    public override void OnExit()
    {
        Debug.Log("exit boost state");
        player.playerInputs.Fishing.CastFishingRod.performed -= player.fishingController.LandPointer;
    }
}
