using UnityEngine;
using Halfmoon.StateMachine;
using JetBrains.Annotations;

public class FishingPullState : BaseState
{
    public FishingPullState(Player player, Animator animator) : base(player, animator) { }

    public override void OnEnter()
    {
        Debug.Log("enter pull state");
    }
    public override void OnExit()
    {
        Debug.Log("exit pull state");
    }
    public override void Update()
    {
        player.fishingController.PullStateUpdateFunction();
        player.fishingController.ControlPullingBar();
    }
}
