using Halfmoon.StateMachine;
using UnityEngine;

public class LocomotionState : BaseState
{
    private PauseViewModel viewModel;
    public LocomotionState(Player player, Animator animator, PauseViewModel viewModel) : base(player, animator)
    {
        this.viewModel = viewModel;
    }
    
    public override void OnEnter()
    {
        //Debug.Log("enter locomotion state");
        player.PlayerInputs.Fishing.Enable();
        player.PlayerInputs.UI.Enable();
        player.PlayerInputs.Fishing.CastFishingRod.performed += player.fishingController.HandleInput;
        player.PlayerInputs.UI.Pause.performed += viewModel.Trigger;
        player.PlayerInputs.Player.Interact.started += player.InteractInput;
        player.PlayerInputs.Player.Interact.canceled += player.InteractInput;
        Debug.Log(InventoryDisplayManager.instance);
        player.PlayerInputs.UI.Inventory.performed += InventoryDisplayManager.instance.ToggleInventory;
    }
    public override void OnExit()
    {
        //Debug.Log("exit locomotion state");
        player.PlayerInputs.Fishing.CastFishingRod.performed -= player.fishingController.HandleInput;
        player.PlayerInputs.UI.Pause.performed -= viewModel.Trigger;
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

    public void Unload()
    {
        player.PlayerInputs.Fishing.CastFishingRod.Dispose();
        player.PlayerInputs.UI.Pause.Dispose();
        player.PlayerInputs.Player.Interact.Dispose();
        player.PlayerInputs.UI.Inventory.Dispose();
    }
}
