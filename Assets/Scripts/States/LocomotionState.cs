using Halfmoon.StateMachine;
using UnityEngine;

public class LocomotionState : BaseState
{
    public LocomotionState(Player player, Animator animator) : base(player, animator) { }
    public override void OnEnter()
    {
        Debug.Log("enter locomotion state");
        player.playerInputs.Fishing.Enable();
        player.playerInputs.UI.Enable();
        player.playerInputs.Fishing.CastFishingRod.performed += player.fishingController.CastOrRetract;
        player.playerInputs.UI.OpenMenu.performed += player.hudController.SwitchMenu;
    }
    public override void OnExit()
    {
        Debug.Log("exit locomotion state");
        player.playerInputs.Fishing.CastFishingRod.performed -= player.fishingController.CastOrRetract;
        player.playerInputs.UI.OpenMenu.performed -= player.hudController.SwitchMenu;
    }
    public override void Update()
    {
        player.HandleMovement();
        player.fishingController.ZoneCheck();
    }
}
