using System.Collections.Generic;
using Stride.Core.Mathematics;
using Test.Game_Logic.Player.AnimationsController;

namespace Test.Game_Logic.Player.PlayerController.StateMachines.States.Locomotion.LocomotionSubstates
{
    public class IdleState : LocomotionState
    {
        public override void Enter(Dictionary<string, object> parameters)
        {
            base.Enter(parameters);
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void Update()
        {
            base.Update();

            Context.DebugText.Print("In Idle state", new Int2(350, 450));

            ShouldMoveToRunning();

            HandleAiming();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void BroadcastAnimationState()
        {
            LocomotionAnimationStateEventKey.Broadcast(LocomotionAnimationState.Idle);
        }

        private void ShouldMoveToRunning()
        {
            if (newMoveDirection.LengthSquared() > float.Epsilon)
            {
                Context.LocomotionStateMachine.TransitionTo(
                    new RunningState(),
                    new Dictionary<string, object> { { "aiming", isAiming } }
                );
            }
        }

        private void HandleAiming()
        {
            if (isAiming)
            {
                Context.DebugText.Print("Strafe Idle", new Int2(350, 350));
            }
            else
            {
                Context.DebugText.Print("Normal Idle", new Int2(350, 350));
            }
        }
    }
}
