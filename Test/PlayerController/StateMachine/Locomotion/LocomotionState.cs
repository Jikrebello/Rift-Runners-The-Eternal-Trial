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
        protected Vector3 moveDirection = Vector3.Zero;
        protected Vector3 currentMoveDirection = Vector3.Zero;
        protected float maxSpeed = 4;
        private float yawOrientation;

        public virtual void Enter(Dictionary<string, object> parameters)
        {
            inputDirectionReceiver = new EventReceiver<Vector3>(PlayerInput.MovementEventKey);
            aimingReceiver = new EventReceiver<bool>(PlayerInput.AimingEventKey);
            if (parameters != null)
            {
                isAiming = (bool)parameters["aiming"];
            }
        }

        public virtual void HandleInput()
        {
            if (inputDirectionReceiver.TryReceive(out Vector3 inputDirection))
            {
                currentMoveDirection = inputDirection;
            }
            else
            {
                currentMoveDirection = Vector3.Zero;
            }

            aimingReceiver.TryReceive(out isAiming);
        }

        public virtual void Update()
        {
            moveDirection = moveDirection * 0.85f + currentMoveDirection * 0.15f;

            Context.Character.SetVelocity(moveDirection * maxSpeed);

            if (moveDirection.Length() > float.Epsilon)
            {
                yawOrientation = MathUtil.RadiansToDegrees(
                    (float)Math.Atan2(-moveDirection.Z, moveDirection.X) + MathUtil.PiOverTwo
                );

                Context.Model.Transform.Rotation = Quaternion.RotationYawPitchRoll(
                    MathUtil.DegreesToRadians(yawOrientation),
                    0,
                    0
                );
            }
        }

        public virtual void Exit()
        {
            // Logic here
        }
    }
}
