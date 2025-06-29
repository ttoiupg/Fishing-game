using UnityEngine;
using Halfmoon.StateMachine;

public class ClearView : ViewState
{
    public ClearView(IViewFrame frame) : base(frame) { }

    public override void OnEnter()
    {
        view.begin();
    }
    public override void OnExit()
    {
        view.End();
    }
}
