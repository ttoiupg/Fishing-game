using Halfmoon.StateMachine;
using UnityEngine;

public class InactiveState : BaseState
{
    public InactiveState(Player player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        player.PlayerInputs.UI.Enable();
        player.PlayerInputs.UI.Menu.performed += player.hudController.SwitchMenu;
        Debug.Log("enter menu open state");
    }

    public override void OnExit()
    {
        player.PlayerInputs.UI.Menu.performed -= player.hudController.SwitchMenu;
        Debug.Log("exit menu open state");
    }
}