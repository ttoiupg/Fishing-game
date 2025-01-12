using Halfmoon.StateMachine;
using UnityEngine;

public class MenuOpenState : BaseState
{
    public MenuOpenState(Player player, Animator animator) : base(player, animator) { }

    public override void OnEnter()
    {
        player.playerInputs.UI.Enable();
        player.playerInputs.UI.OpenMenu.performed += player.hudController.SwitchMenu;
        Debug.Log("enter menu open state");
    }

    public override void OnExit()
    {
        player.playerInputs.UI.OpenMenu.performed -= player.hudController.SwitchMenu;
        Debug.Log("exit menu open state");
    }
    public override void Update()
    {
        player.HandleMovement();
        player.fishingController.ZoneCheck();
    }
}