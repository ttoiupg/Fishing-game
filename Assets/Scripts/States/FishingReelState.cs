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
        VibrationHandler.Instance.SetMotorSpeed(0, 0);
    }
    public override void Update()
    {
        if (!GameManager.Instance.CurrentBattle.battleStarted) return;
        player.ReelCanvaManager.UpdatePosition();
        player.ReelCanvaManager.UpdateWarningScreen();
        player.fishingController.ControlReelingBar();
        player.fishingController.ReelStateUpdateFunction();
        player.ReelCanvaManager.GamepadVibration();
    }
}
