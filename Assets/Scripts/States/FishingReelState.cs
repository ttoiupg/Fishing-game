using Halfmoon.StateMachine;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingReelState : BaseState
{
    public FishingReelState(Player player, Animator animator) : base(player, animator) { }

    public override void OnEnter()
    {
        player.ReelCanvaManager.Init();
    }
    public override void OnExit()
    {
        player.ReelCanvaManager.CloseUI();
        if(Gamepad.current != null) {
            if(Gamepad.current?.name != "DualShock4GamepadHID") {
                Gamepad.current?.SetMotorSpeeds(0, 0);
            }
        }
    }
    public override void Update()
    {
        if (!GameManager.Instance.CurrentBattle.battleStarted) return;
        player.ReelCanvaManager.UpdatePosition();
        player.fishingController.ControlReelingBar();
        player.fishingController.ReelStateUpdateFunction();
        player.ReelCanvaManager.GamepadVibration();
    }
}
