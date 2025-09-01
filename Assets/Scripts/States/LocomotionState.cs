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
        player.PlayerInputs.UI.Pause.performed += PauseViewModel.Instance.Trigger;
        player.PlayerInputs.Player.Interact.started += player.InteractInput;
        player.PlayerInputs.Player.Interact.canceled += player.InteractInput;
        Debug.Log(InventoryDisplayManager.instance);
        player.PlayerInputs.UI.Inventory.performed += InventoryDisplayManager.instance.ToggleInventory;
    }
    public override void OnExit()
    {
        //Debug.Log("exit locomotion state");
        player.PlayerInputs.Fishing.CastFishingRod.performed -= player.fishingController.HandleInput;
        player.PlayerInputs.UI.Pause.performed -= PauseViewModel.Instance.Trigger;
        player.PlayerInputs.Player.Interact.started -= player.InteractInput;
        player.PlayerInputs.Player.Interact.canceled -= player.InteractInput;
        player.PlayerInputs.UI.Inventory.performed -= InventoryDisplayManager.instance.ToggleInventory;
    }
    public override void Update()
    {
        player.UpdateInteraction();
    }

    public override void FixedUpdate()
    {
        player.UpdateMovement();
    }
}
