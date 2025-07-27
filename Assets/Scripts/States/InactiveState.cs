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
        player.PlayerInputs.UI.Pause.performed += PauseViewModel.Instance.Trigger;
        player.PlayerInputs.UI.Inventory.performed += InventoryDisplayManager.instance.ToggleInventory;
        player.currentInteract?.PromptHide(player);
        Debug.Log("enter menu open state");
    }

    public override void OnExit()
    {
        player.PlayerInputs.UI.Pause.performed -= PauseViewModel.Instance.Trigger;
        player.PlayerInputs.UI.Inventory.performed -= InventoryDisplayManager.instance.ToggleInventory;
        Debug.Log("exit menu open state");
    }
}