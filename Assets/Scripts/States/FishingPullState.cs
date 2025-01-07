using UnityEngine;
using Halfmoon.StateMachine;
using JetBrains.Annotations;

public class FishingPullState : BaseState
{
    public FishingPullState(Player player, Animator animator) : base(player, animator) { }

    public override void OnEnter()
    {
        player.pullCanvaManager.Init();
        Debug.Log("enter pull state");
    }
    public override void OnExit()
    {
        player.pullCanvaManager.CloseUI();
        Debug.Log("exit pull state");
    }
    public override void Update()
    {
        player.pullCanvaManager.UpdatePosition();
        player.fishingController.ControlPullingBar();
        player.fishingController.PullStateUpdateFunction();
        player.pullCanvaManager.GamepadVibration();
    }
}
