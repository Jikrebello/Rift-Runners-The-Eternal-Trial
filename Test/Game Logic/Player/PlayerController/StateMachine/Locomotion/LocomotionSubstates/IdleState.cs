using System.Collections.Generic;
using Stride.Core.Mathematics;

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

            if (aimingReceiver.TryReceive(out bool aiming))
            {
                isAiming = aiming;
            }
        }

        public override void Update()
        {
            // Logic here
            base.Update();
            Context.ScriptComponent.DebugText.Print("In Idle state", new Int2(350, 450));

            if (currentMoveDirection.LengthSquared() > float.Epsilon)
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
