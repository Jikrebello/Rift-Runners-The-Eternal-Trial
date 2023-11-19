using Stride.Core.Mathematics;
using Stride.Engine.Events;
using Stride.Input;
using System;
using System.Collections.Generic;

namespace Test.PlayerController.StateMachine.Locomotion
{
    public class LocomotionState : IState
    {
        public PlayerContext Context { get; set; }

        protected EventReceiver<bool> aimingReceiver;
        protected bool isAiming;
        protected Vector3 inputDirection;
        protected Vector3 moveDirection;
        protected float maxSpeed = 4;
        protected Quaternion previousRotation = Quaternion.Identity;
        protected float rotationSpeed = 10;

        public virtual void Enter(Dictionary<string, object> parameters)
        {
            aimingReceiver = new EventReceiver<bool>(Context.AimingEventKey);
            if (parameters != null)
            {
                isAiming = (bool)parameters["aiming"];
            }
        }

        public virtual void HandleInput() { }

        public virtual void Update()
        {
            inputDirection = GetInputDirection();
            moveDirection = inputDirection * maxSpeed;

            Context.Character.SetVelocity(moveDirection);

            if (moveDirection.Length() > float.Epsilon)
            {
                Context.DebugText.Print($"Move Direction: {moveDirection}", new Int2(250, 250));

                Context.Model.Transform.Rotation = LookAt(moveDirection, rotationSpeed);
            }
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

        private Quaternion LookAt(Vector3 moveDirection, float interpolationSpeed)
        {
            if (moveDirection.LengthSquared() < float.Epsilon)
                return previousRotation;

            float yawOrientation = MathUtil.RadiansToDegrees(
                (float)Math.Atan2(-moveDirection.Z, moveDirection.X) + MathUtil.PiOverTwo
            );

            Quaternion targetRotation = Quaternion.RotationYawPitchRoll(
                MathUtil.DegreesToRadians(yawOrientation),
                0,
                0
            );

            float slerpFactor = Math.Min(
                interpolationSpeed * (float)Context.Game.UpdateTime.Elapsed.TotalSeconds,
                1.0f
            );
            Quaternion interpolatedRotation = Quaternion.Slerp(
                previousRotation,
                targetRotation,
                slerpFactor
            );

            previousRotation = interpolatedRotation;

            return interpolatedRotation;
        }
    }
}
