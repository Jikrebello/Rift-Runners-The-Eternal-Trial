using System;
using System.Collections.Generic;
using Stride.Core.Mathematics;
using Test.Game_Logic.Player.AnimationsController;

namespace Test.Game_Logic.Player.PlayerController.StateMachines.States.Locomotion.LocomotionSubstates
{
    public class RunningState : LocomotionState
    {
        private readonly float forwardsAimingSpeed = 4.5f;
        private readonly float horizontalAimingSpeed = 4.8f;
        private readonly float backwardsAimingSpeed = 3.5f;

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

            Context.ScriptComponent.DebugText.Print("In Running state", new Int2(350, 450));

            ShouldMoveToIdle(currentMoveDirection.LengthSquared() <= float.Epsilon);

            ProcessAimingDirection();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void BroadcastAnimationState()
        {
            LocomotionAnimationStateEventKey.Broadcast(LocomotionAnimationState.Running);
        }

        private void ShouldMoveToIdle(bool isMoving)
        {
            if (isMoving)
            {
                Context.LocomotionStateMachine.TransitionTo(
                    new IdleState(),
                    new Dictionary<string, object> { { "aiming", isAiming } }
                );
            }
        }

        private void ProcessAimingDirection()
        {
            if (isAiming)
            {
                if (
                    Math.Abs(relativeMovementDirection.X) > 0.1f
                    && relativeMovementDirection.Z > 0.1f
                )
                {
                    string diagonalDirection = relativeMovementDirection.X > 0 ? "Left" : "Right";
                    Context.ScriptComponent.DebugText.Print(
                        $"Diagonal Strafing {diagonalDirection} Forwards",
                        new Int2(350, 350)
                    );
                    SetCharacterVelocity(forwardsAimingSpeed);
                }
                else if (
                    Math.Abs(relativeMovementDirection.X) > 0.1f
                    && relativeMovementDirection.Z < -0.1f
                )
                {
                    string diagonalDirection = relativeMovementDirection.X > 0 ? "Left" : "Right";
                    Context.ScriptComponent.DebugText.Print(
                        $"Diagonal Strafing {diagonalDirection} Backwards",
                        new Int2(350, 350)
                    );
                    SetCharacterVelocity(backwardsAimingSpeed);
                }
                else if (Math.Abs(relativeMovementDirection.X) > 0.1f)
                {
                    string strafeDirectionDescription =
                        relativeMovementDirection.X > 0 ? "Left" : "Right";
                    Context.ScriptComponent.DebugText.Print(
                        $"Horizontal Strafing {strafeDirectionDescription}",
                        new Int2(350, 350)
                    );
                    SetCharacterVelocity(horizontalAimingSpeed);
                }
                else if (relativeMovementDirection.Z > 0.1f)
                {
                    Context.ScriptComponent.DebugText.Print(
                        "Strafing Forwards",
                        new Int2(350, 350)
                    );
                    SetCharacterVelocity(forwardsAimingSpeed);
                }
                else if (relativeMovementDirection.Z < -0.1f)
                {
                    Context.ScriptComponent.DebugText.Print(
                        "Strafing Backward",
                        new Int2(350, 350)
                    );
                    SetCharacterVelocity(backwardsAimingSpeed);
                }
            }
            else
            {
                Context.ScriptComponent.DebugText.Print("Normal Running", new Int2(350, 350));
            }
        }
    }
}
