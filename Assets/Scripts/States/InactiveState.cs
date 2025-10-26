using Halfmoon.StateMachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InactiveState : BaseState
{
    private PauseViewModel view;
    public InactiveState(Player player, Animator animator,PauseViewModel view) : base(player, animator)
    {
        this.view = view;
    }

    public override void OnEnter()
    {
        player.PlayerInputs.UI.Enable();
        player.PlayerInputs.UI.Pause.performed += view.Trigger;
        player.PlayerInputs.UI.Inventory.performed += InventoryDisplayManager.instance.ToggleInventory;
        player.currentInteract?.PromptHide(player);
        Debug.Log("enter menu open state");
    }

    public override void OnExit()
    {
        player.PlayerInputs.UI.Pause.performed -= view.Trigger;
        player.PlayerInputs.UI.Inventory.performed -= InventoryDisplayManager.instance.ToggleInventory;
        Debug.Log("exit menu open state");
    }

    public void Unload()
    {
        player.PlayerInputs.UI.Pause.Dispose();
        player.PlayerInputs.UI.Inventory.Dispose();
    }
}