using Halfmoon.StateMachine;
using UnityEngine;

public class LocomotionState : BaseState
{
    public LocomotionState(Player player, Animator animator) : base(player, animator) { }

    public override void OnEnter()
    {
        //Debug.Log("enter locomotion state");
        player.PlayerInputs.Fishing.Enable();
        player.PlayerInputs.UI.Enable();
        player.PlayerInputs.Fishing.CastFishingRod.performed += player.fishingController.HandleInput;
        player.PlayerInputs.UI.OpenMenu.performed += player.hudController.SwitchMenu;
    }
    public override void OnExit()
    {
        //Debug.Log("exit locomotion state");
        player.PlayerInputs.Fishing.CastFishingRod.performed -= player.fishingController.HandleInput;
        player.PlayerInputs.UI.OpenMenu.performed -= player.hudController.SwitchMenu;
    }
    public override void Update()
    {
        player.UpdateInteraction();
        player.UpdateMovement();
        player.fishingController.ZoneCheck();

    }
}
