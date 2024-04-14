using System.Collections.Generic;
using Stride.Engine.Events;
using Test.Game_Logic.Player.AnimationsController;

namespace Test.Game_Logic.Player.PlayerController.StateMachines.States.Combat
{
    public class CombatState : IState
    {
        public PlayerContext Context { get; set; }

        protected EventReceiver<bool> aimingReceiver;
        protected bool isAiming;

        public virtual void Enter(Dictionary<string, object> parameters)
        {
            // Logic here
            aimingReceiver = new EventReceiver<bool>(PlayerInput.AimingEventKey);
            if (parameters != null)
            {
                isAiming = (bool)parameters["aiming"];
            }
        }

        public virtual void BroadcastAnimationState()
        {
            throw new System.NotImplementedException();
        }

        public virtual void HandleInput()
        {
            // Logic here
            if (aimingReceiver.TryReceive(out bool aiming))
            {
                isAiming = aiming;
            }
        }

        public virtual void Update()
        {
            // Logic here
        }

        public virtual void Exit()
        {
            // Logic here
        }
    }
}
