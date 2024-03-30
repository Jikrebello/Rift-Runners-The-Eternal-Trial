using System;
using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Engine.Events;

namespace Test.PlayerController.StateMachine.Locomotion
{
    public class LocomotionState : IState
    {
        public PlayerContext Context { get; set; }

        protected EventReceiver<bool> aimingReceiver;
        protected EventReceiver<Vector3> inputDirectionReceiver;
        protected EventReceiver<Vector3> cameraForwardReceiver;
        protected bool isMoving;
        protected bool isAiming;
        protected Vector3 moveDirection = Vector3.Zero;
        protected Vector3 currentMoveDirection = Vector3.Zero;
        protected float maxSpeed = 4;
        protected Vector3 relativeMovementDirection;

        // Controls smoothing of the movement velocity.
        private const float _VELOCITY_SMOOTH_FACTOR = 0.85f;

        // Controls how responsive the movement is to input changes.
        private const float _INPUT_RESPONSE_FACTOR = 0.15f;

        private float _yawOrientation;

        public virtual void Enter(Dictionary<string, object> parameters)
        {
            inputDirectionReceiver = new EventReceiver<Vector3>(PlayerInput.MovementEventKey);

            aimingReceiver = new EventReceiver<bool>(PlayerInput.AimingEventKey);
            if (parameters != null)
            {
                isAiming = (bool)parameters["aiming"];
            }

            cameraForwardReceiver = new EventReceiver<Vector3>(PlayerInput.CameraForwardEventKey);
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
            AdjustPlayerVelocityBasedOnInput();

            if (isAiming)
            {
                HandleAimingOrientation();
            }
            else
            {
                UpdatePlayerRotationBasedOnMovement();
            }
        }

        public virtual void Exit()
        {
            // Logic here
        }

        protected void AdjustPlayerVelocityBasedOnInput()
        {
            moveDirection =
                moveDirection * _VELOCITY_SMOOTH_FACTOR
                + currentMoveDirection * _INPUT_RESPONSE_FACTOR;
            Context.Character.SetVelocity(moveDirection * maxSpeed);
        }

        protected void HandleAimingOrientation()
        {
            if (cameraForwardReceiver.TryReceive(out Vector3 cameraForward))
            {
                // Normalize and adjust camera forward direction if necessary
                cameraForward.Y = 0;
                cameraForward.Normalize();

                // Align the character's forward direction with the camera's
                var newOrientation = Quaternion.LookRotation(cameraForward, Vector3.UnitY);
                Context.Model.Transform.Rotation = newOrientation;

                // Calculate relative movement direction
                var rotationMatrix = Matrix.RotationQuaternion(newOrientation);
                relativeMovementDirection = Vector3.TransformNormal(
                    currentMoveDirection,
                    Matrix.Invert(rotationMatrix)
                );

                relativeMovementDirection.Normalize();
            }
        }

        protected void UpdatePlayerRotationBasedOnMovement()
        {
            if (moveDirection.LengthSquared() > float.Epsilon)
            {
                _yawOrientation = MathUtil.RadiansToDegrees(
                    (float)Math.Atan2(-moveDirection.Z, moveDirection.X) + MathUtil.PiOverTwo
                );

                Context.Model.Transform.Rotation = Quaternion.RotationYawPitchRoll(
                    MathUtil.DegreesToRadians(_yawOrientation),
                    0,
                    0
                );
            }
        }
    }
}
