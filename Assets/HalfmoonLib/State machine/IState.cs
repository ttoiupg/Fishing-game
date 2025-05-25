using UnityEngine;

namespace Halfmoon.StateMachine
{
    public interface IState
    {
        //when transition from another state
        void OnEnter();
        //normal mono update
        void Update();
        //for physic
        void FixedUpdate();
        //when transition to another state
        void OnExit();
    }
}
