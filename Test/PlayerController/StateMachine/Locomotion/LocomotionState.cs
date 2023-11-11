using Stride.Core.Mathematics;
using Stride.Engine.Events;
using Stride.Input;
using System;
using System.Collections.Generic;

namespace Test.PlayerController.StateMachine.Locomotion
{
    public class LocomotionState : IState
    {
        protected EventReceiver<bool> aimingReceiver;
        protected bool isAiming;
        protected Vector3 inputDirection;
        protected Vector3 moveDirection;
        protected float maxSpeed = 2;
        public PlayerContext Context { get; set; }

        public virtual void Enter(Dictionary<string, object> parameters)
        {
            // Logic here
            aimingReceiver = new EventReceiver<bool>(Context.AimingEventKey);
            if (parameters != null)
            {
                isAiming = (bool)parameters["aiming"];
            }
        }

        public virtual void HandleInput()
        {
            // Logic here
        }

        public virtual void Update()
        {
            // Logic here
            Context.ScriptComponent.DebugText.Print(
                $"Input Direction: {GetInputDirection()}",
                new Int2(250, 250)
            );

            inputDirection = GetInputDirection();

            moveDirection = inputDirection * maxSpeed;

            Context.Character.SetVelocity(moveDirection);
        }

        public virtual void Exit()
        {
            // Logic here
        }

        private Vector3 GetInputDirection()
        {
            Vector3 inputDirection =
                new(
                    (Context.Input.IsKeyDown(Keys.A) ? 1 : 0)
                        - (Context.Input.IsKeyDown(Keys.D) ? 1 : 0),
                    0.0f,
                    (Context.Input.IsKeyDown(Keys.W) ? 1 : 0)
                        - (Context.Input.IsKeyDown(Keys.S) ? 1 : 0)
                );

            inputDirection.Normalize();

            return inputDirection;
        }

        private Quaternion LookAt(Vector3 moveDirection)
        {
            float yawOrientation = 0;

            if (moveDirection.Length() > 0.001)
            {
                yawOrientation = MathUtil.RadiansToDegrees(
                    (float)Math.Atan2(-moveDirection.Z, moveDirection.X) + MathUtil.PiOverTwo
                );
            }

            return Quaternion.RotationYawPitchRoll(MathUtil.DegreesToRadians(yawOrientation), 0, 0);
        }
    }
}
