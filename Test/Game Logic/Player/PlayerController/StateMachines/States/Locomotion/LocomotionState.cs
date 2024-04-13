using System;
using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Engine.Events;
using Test.Game_Logic.Player.AnimationsController;

namespace Test.Game_Logic.Player.PlayerController.StateMachines.States.Locomotion
{
    public class LocomotionState : IState
    {
        public PlayerContext Context { get; set; }

        public static readonly EventKey<Vector3> RelativeMovementDirectionEventKey =
            new("Player Event", "RelativeMovementDirection");
        public static readonly EventKey<float> PlayerSpeedEventKey =
            new("Player Event", "PlayerSpeed");
        protected EventKey<LocomotionAnimationState> LocomotionAnimationStateEventKey =
            new("Player Event", "Locomotion Animation State");

        protected Vector3 currentMoveDirection = Vector3.Zero;
        protected Vector3 newMoveDirection = Vector3.Zero;
        protected Vector3 relativeMovementDirection;
        protected bool isAiming;

        private EventReceiver<Vector3> _newInputDirectionReceiver;
        private EventReceiver<Vector3> _cameraForwardReceiver;
        private EventReceiver<bool> _aimingReceiver;

        private const float _VELOCITY_SMOOTH_FACTOR = 0.85f;
        private const float _INPUT_RESPONSE_FACTOR = 0.15f;
        private readonly float _normalMoveSpeed = 6;

        private bool _isMoving;
        private float _effectiveSpeed;
        private float _currentSpeed;
        private float _yawOrientation;

        public virtual void Enter(Dictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                isAiming = (bool)parameters["aiming"];
            }

            _newInputDirectionReceiver = new EventReceiver<Vector3>(
                PlayerInput.NewInputDirectionEventKey
            );
            _cameraForwardReceiver = new EventReceiver<Vector3>(PlayerInput.CameraForwardEventKey);
            _aimingReceiver = new EventReceiver<bool>(PlayerInput.AimingEventKey);
        }

        public virtual void HandleInput()
        {
            // Only handle movement when Grounded
            if (Context.Character.IsGrounded)
            {
                if (_newInputDirectionReceiver.TryReceive(out Vector3 newInputDirection))
                {
                    newMoveDirection = newInputDirection;
                }
                else
                {
                    newMoveDirection = Vector3.Zero;
                }
                _aimingReceiver.TryReceive(out isAiming);
            }
        }

        public virtual void Update()
        {
            CalculateCurrentMovementDirection();

            if (isAiming)
            {
                HandleAimingOrientation();
            }
            else
            {
                UpdatePlayerRotationBasedOnMovement();
            }

            // Only handle movement when Grounded
            if (Context.Character.IsGrounded)
            {
                SetCharacterVelocity();
            }
        }

        public virtual void Exit() { }

        public virtual void BroadcastAnimationState() { }

        private void CalculateCurrentMovementDirection()
        {
            currentMoveDirection =
                currentMoveDirection * _VELOCITY_SMOOTH_FACTOR
                + newMoveDirection * _INPUT_RESPONSE_FACTOR;
        }

        private void HandleAimingOrientation()
        {
            if (_cameraForwardReceiver.TryReceive(out Vector3 cameraForward))
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
                    newMoveDirection,
                    Matrix.Invert(rotationMatrix)
                );

                relativeMovementDirection.Normalize();
                RelativeMovementDirectionEventKey.Broadcast(relativeMovementDirection);
            }
        }

        private void UpdatePlayerRotationBasedOnMovement()
        {
            if (currentMoveDirection.LengthSquared() > float.Epsilon)
            {
                _yawOrientation = MathUtil.RadiansToDegrees(
                    (float)Math.Atan2(-currentMoveDirection.Z, currentMoveDirection.X)
                        + MathUtil.PiOverTwo
                );

                Context.Model.Transform.Rotation = Quaternion.RotationYawPitchRoll(
                    MathUtil.DegreesToRadians(_yawOrientation),
                    0,
                    0
                );
            }
        }

        protected void SetCharacterVelocity(float variableSpeed = 0f)
        {
            // Determine the effective move speed.
            _effectiveSpeed = variableSpeed > 0f ? variableSpeed : _normalMoveSpeed;

            // Set the character's velocity based on the effective move speed.
            Context.Character.SetVelocity(currentMoveDirection * _effectiveSpeed);

            // Update the current speed for broadcasting.
            UpdateAndBroadcastPlayerSpeed(_effectiveSpeed);
        }

        private void UpdateAndBroadcastPlayerSpeed(float effectiveSpeed)
        {
            _isMoving = currentMoveDirection != Vector3.Zero;

            if (_isMoving)
            {
                _currentSpeed = Math.Min(effectiveSpeed / _normalMoveSpeed, 1.0f);
            }
            else
            {
                _currentSpeed = 0.0f;
            }

            PlayerSpeedEventKey.Broadcast(_currentSpeed);
        }
    }
}
