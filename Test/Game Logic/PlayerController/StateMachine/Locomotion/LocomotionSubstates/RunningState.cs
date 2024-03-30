using System.Collections.Generic;
using Stride.Core.Mathematics;

namespace Test.PlayerController.StateMachine.Locomotion
{
    public class RunningState : LocomotionState
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
            base.Update(); // Logic here
            Context.ScriptComponent.DebugText.Print("In Running state", new Int2(350, 450));

            if (currentMoveDirection.LengthSquared() <= float.Epsilon)
            {
                Context.LocomotionStateMachine.TransitionTo(
                    new IdleState(),
                    new Dictionary<string, object> { { "aiming", isAiming } }
                );
            }

            if (isAiming)
            {
                Context.ScriptComponent.DebugText.Print("Strafe Running", new Int2(350, 350));
            }
            else
            {
                Context.ScriptComponent.DebugText.Print("Normal Running", new Int2(350, 350));
            }
        }

        public override void Exit()
        {
            // Logic here
        }
    }
}
