using UnityEngine;
using Halfmoon.StateMachine;
using JetBrains.Annotations;

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
