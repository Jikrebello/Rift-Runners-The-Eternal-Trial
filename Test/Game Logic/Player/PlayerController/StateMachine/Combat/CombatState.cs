using System.Collections.Generic;
using Stride.Engine.Events;

namespace Test.PlayerController.StateMachine.Combat
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
            //Context.ScriptComponent.DebugText.Print("In Combat state", new Int2(550, 350));

            if (isAiming)
            {
                Context.CombatStateMachine.TransitionTo(
                    new AimingState(),
                    new Dictionary<string, object> { { "aiming", isAiming } }
                );
            }
        }

        public virtual void Exit()
        {
            // Logic here
        }
    }
}
