using UnityEngine;

namespace Halfmoon.StateMachine
{
    public abstract class BaseState : IState
    {
        protected readonly Player player;
        protected readonly Animator animator;
        
        protected BaseState(Player player, Animator animator)
        {
            this.player = player;
            this.animator = animator;
        }

        public virtual void OnEnter()
        {

        }
        public virtual void Update()
        {

        }
        public virtual void FixedUpdate()
        { 

        }

        public virtual void OnExit()
        {
        
        }
    }
}
