using Halfmoon.StateMachine;
using UnityEngine;

public class LocomotionState : BaseState
{
    public LocomotionState(Player player, Animator animator) : base(player, animator) { }
    public override void OnEnter()
    {
        Debug.Log("enter locomotion state");
        player.playerInputs.Fishing.Enable();
        player.playerInputs.Fishing.CastFishingRod.performed += player.fishingController.CastOrRetract;
    }
    public override void OnExit()
    {
        Debug.Log("exit locomotion state");
        player.playerInputs.Fishing.CastFishingRod.performed -= player.fishingController.CastOrRetract;
    }
    public override void FixedUpdate()
    {
        player.HandleMovement();
        player.fishingController.ZoneCheck();
    }
}
