using Stride.Core.Mathematics;
using System.Collections.Generic;

namespace Test.PlayerController.StateMachine.Locomotion
{
    public class IdleState : LocomotionState
    {
        public override void Enter(Dictionary<string, object> parameters)
        {
            // Logic here
            base.Enter(parameters);
        }

        public override void HandleInput()
        {
            // Logic here
            base.HandleInput();
        }

        public override void Update()
        {
            base.Update(); // Logic here
            Context.ScriptComponent.DebugText.Print("In Idle state", new Int2(350, 450));

            if (aimingReceiver.TryReceive(out bool aiming))
            {
                isAiming = aiming;
            }

            if (currentMoveDirection.LengthSquared() > 0f)
            {
                Context.LocomotionStateMachine.TransitionTo(
                    new RunningState(),
                    new Dictionary<string, object> { { "aiming", isAiming } }
                );
            }

            if (isAiming)
            {
                Context.ScriptComponent.DebugText.Print("Strafe Idle", new Int2(350, 350));
            }
            else
            {
                Context.ScriptComponent.DebugText.Print("Normal Idle", new Int2(350, 350));
            }
        }

        public override void Exit()
        {
            // Logic here
        }
    }
}
