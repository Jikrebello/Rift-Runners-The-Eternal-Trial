using Stride.Core.Mathematics;
using Stride.Engine.Events;
using System;
using System.Collections.Generic;

namespace Test.PlayerController.StateMachine.Locomotion
{
    public class LocomotionState : IState
    {
        public PlayerContext Context { get; set; }

        protected EventReceiver<bool> aimingReceiver;
        protected EventReceiver<Vector3> inputDirectionReceiver;
        protected bool isMoving;
        protected bool isAiming;
        protected Vector3 moveDirection;
        protected float maxSpeed = 4;
        protected Quaternion previousRotation = Quaternion.Identity;
        protected float rotationSpeed = 10;

        public virtual void Enter(Dictionary<string, object> parameters)
        {
            inputDirectionReceiver = new EventReceiver<Vector3>(PlayerInput.InputDirectionEventKey);
            aimingReceiver = new EventReceiver<bool>(Context.AimingEventKey);
            if (parameters != null)
            {
                isAiming = (bool)parameters["aiming"];
            }
        }

        public virtual void HandleInput()
        {
            if (inputDirectionReceiver.TryReceive(out Vector3 inputDirection))
            {
                isMoving = true;
                moveDirection = inputDirection;
            }
            else
            {
                isMoving = false;
            }

            if (aimingReceiver.TryReceive(out bool aiming))
            {
                isAiming = aiming;
            }
        }

        public virtual void Update()
        {
            if (isMoving)
            {
                // Apply inertia for fluid motion
                moveDirection = moveDirection * 0.85f + moveDirection * 0.15f;
                Context.Character.SetVelocity(moveDirection * maxSpeed);

                // Handle rotation
                if (moveDirection.LengthSquared() > 0.001f)
                {
                    float yawOrientation = MathUtil.RadiansToDegrees(
                        (float)Math.Atan2(-moveDirection.Z, moveDirection.X) + MathUtil.PiOverTwo
                    );
                    Context.Model.Transform.Rotation = Quaternion.RotationYawPitchRoll(
                        MathUtil.DegreesToRadians(yawOrientation),
                        0,
                        0
                    );
                }
            }
            else
            {
                Context.Character.SetVelocity(Vector3.Zero);
            }
        }

        public virtual void Exit()
        {
            // Logic here
        }
    }
}
