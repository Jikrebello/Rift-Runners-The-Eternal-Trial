﻿using System;
using System.Collections.Generic;
using Stride.Core.Mathematics;

namespace Test.PlayerController.StateMachine.Locomotion
{
    public class RunningState : LocomotionState
    {
        private readonly float forwardsAimingMaxSpeed = 4.5f;
        private readonly float horizontalAimingSpeed = 4.8f;
        private readonly float backwardsAimingMaxSpeed = 3.5f;

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
            Context.ScriptComponent.DebugText.Print("In Running state", new Int2(350, 450));

            ShouldMoveToIdle(currentMoveDirection.LengthSquared() <= float.Epsilon);

            ProcessAimingDirection();
        }

        public override void Exit()
        {
            // Logic here
        }

        private void ShouldMoveToIdle(bool isCurrentMoveDirectionGreaterThanZero)
        {
            if (isCurrentMoveDirectionGreaterThanZero)
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
                    string diagonalDirection = relativeMovementDirection.X > 0 ? "Right" : "Left";
                    Context.ScriptComponent.DebugText.Print(
                        $"Diagonal Strafing {diagonalDirection} Forwards",
                        new Int2(350, 350)
                    );
                    Context.Character.SetVelocity(moveDirection * forwardsAimingMaxSpeed);
                }
                else if (
                    Math.Abs(relativeMovementDirection.X) > 0.1f
                    && relativeMovementDirection.Z < -0.1f
                )
                {
                    string diagonalDirection = relativeMovementDirection.X > 0 ? "Right" : "Left";
                    Context.ScriptComponent.DebugText.Print(
                        $"Diagonal Strafing {diagonalDirection} Backwards",
                        new Int2(350, 350)
                    );
                    Context.Character.SetVelocity(moveDirection * backwardsAimingMaxSpeed);
                }
                else if (Math.Abs(relativeMovementDirection.X) > 0.1f)
                {
                    string strafeDirectionDescription =
                        relativeMovementDirection.X > 0 ? "Right" : "Left";
                    Context.ScriptComponent.DebugText.Print(
                        $"Horizontal Strafing {strafeDirectionDescription}",
                        new Int2(350, 350)
                    );
                    Context.Character.SetVelocity(moveDirection * horizontalAimingSpeed);
                }
                else if (relativeMovementDirection.Z > 0.1f)
                {
                    Context.ScriptComponent.DebugText.Print(
                        "Strafing Forwards",
                        new Int2(350, 350)
                    );
                    Context.Character.SetVelocity(moveDirection * forwardsAimingMaxSpeed);
                }
                else if (relativeMovementDirection.Z < -0.1f)
                {
                    Context.ScriptComponent.DebugText.Print(
                        "Strafing Backward",
                        new Int2(350, 350)
                    );
                    Context.Character.SetVelocity(moveDirection * backwardsAimingMaxSpeed);
                }
            }
            else
            {
                Context.ScriptComponent.DebugText.Print("Normal Running", new Int2(350, 350));
            }
        }
    }
}
