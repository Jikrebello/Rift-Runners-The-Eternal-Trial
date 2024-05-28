using System;
using System.Collections.Generic;
using System.Linq;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Input;

namespace Test.Game_Logic.Player.PlayerController
{
    public class PlayerInput : SyncScript
    {
        public static readonly EventKey<Vector3> NewInputDirectionEventKey =
            new("Player Event", "Input Movement");
        public static readonly EventKey<Vector2> CameraRotateEventKey =
            new("Camera Events", "Camera Rotate");
        public static readonly EventKey<Vector3> CameraForwardEventKey =
            new("Camera Events", "Camera Forward");
        public static readonly EventKey<bool> PlayerJumpEventKey = new("Player Event", "Jumping");
        public static readonly EventKey<bool> AimingEventKey = new("Player Event", "Aiming");

        public CameraComponent PlayerCamera { get; set; }
        public float MouseSensitivity = 1f;

        public List<Keys> MoveUpKeys { get; } = [];
        public List<Keys> MoveLeftKeys { get; } = [];
        public List<Keys> MoveDownKeys { get; } = [];
        public List<Keys> MoveRightKeys { get; } = [];
        public List<MouseButton> AimButtons { get; set; } = [];
        public List<Keys> JumpKeys { get; } = [];

        private Vector2 _inputDirectionFromControls = Vector2.Zero;
        private Vector3 _worldDirection = Vector3.Zero;
        private Vector2 _cameraRotationInput = Vector2.Zero;
        private bool _wasJumpKeyPressedLastFrame = false;

        public override void Start()
        {
            base.Start();
            AssignKeys();
        }

        private void AssignKeys()
        {
            MoveUpKeys.Add(Keys.W);
            MoveLeftKeys.Add(Keys.A);
            MoveDownKeys.Add(Keys.S);
            MoveRightKeys.Add(Keys.D);

            AimButtons.Add(MouseButton.Right);

            JumpKeys.Add(Keys.Space);
        }

        public override void Update()
        {
            HandleCameraMovement();

            BroadcastPlayerAimingEventKey();

            BroadcastCameraForwardDirection();

            BroadcastPlayerJumpEventKey();
        }

        #region Camera Movement
        private void HandleCameraMovement()
        {
            HandleInputMovementDirectionAndBroadcastWorldDirection();
            HandleCameraRotationInputAndBroadcast();
            HandleCursorLocking();
        }

        private void HandleInputMovementDirectionAndBroadcastWorldDirection()
        {
            _inputDirectionFromControls = Vector2.Zero;

            if (MoveLeftKeys.Any(Input.IsKeyDown))
                _inputDirectionFromControls += -Vector2.UnitX;
            if (MoveRightKeys.Any(Input.IsKeyDown))
                _inputDirectionFromControls += Vector2.UnitX;
            if (MoveUpKeys.Any(Input.IsKeyDown))
                _inputDirectionFromControls += Vector2.UnitY;
            if (MoveDownKeys.Any(Input.IsKeyDown))
                _inputDirectionFromControls += -Vector2.UnitY;

            _worldDirection = ConvertInputDirectionToWorldDirection(_inputDirectionFromControls);

            NewInputDirectionEventKey.Broadcast(_worldDirection);
        }

        private Vector3 ConvertInputDirectionToWorldDirection(Vector2 movementInputDirection)
        {
            if (PlayerCamera == null)
            {
                throw new InvalidOperationException(
                    "PlayerCamera is not set. Please assign a CameraComponent to PlayerCamera."
                );
            }

            PlayerCamera.Update();
            var inverseView = Matrix.Invert(PlayerCamera.ViewMatrix);

            var forward = Vector3.Cross(Vector3.UnitY, inverseView.Right);
            forward.Normalize();

            var right = Vector3.Cross(forward, Vector3.UnitY);
            right.Normalize();

            var worldDirection =
                forward * movementInputDirection.Y + right * movementInputDirection.X;
            worldDirection.Normalize();
            return worldDirection;
        }

        private void HandleCameraRotationInputAndBroadcast()
        {
            _cameraRotationInput =
                new Vector2(Input.MouseDelta.X, -Input.MouseDelta.Y) * MouseSensitivity;

            CameraRotateEventKey.Broadcast(_cameraRotationInput);
        }

        private void HandleCursorLocking()
        {
            if (Input.IsMouseButtonPressed(MouseButton.Left))
            {
                Input.LockMousePosition(true);
                Game.IsMouseVisible = false;
            }
            else if (Input.IsKeyPressed(Keys.Escape))
            {
                Input.UnlockMousePosition();
                Game.IsMouseVisible = true;
            }
        }
        #endregion

        private void BroadcastPlayerAimingEventKey()
        {
            if (AimButtons.Any(Input.IsMouseButtonDown))
            {
                AimingEventKey.Broadcast(true);
            }
            else
            {
                AimingEventKey.Broadcast(false);
            }
        }

        private void BroadcastCameraForwardDirection()
        {
            if (PlayerCamera != null)
            {
                var cameraForward = PlayerCamera.Entity.Transform.WorldMatrix.Forward;

                CameraForwardEventKey.Broadcast(cameraForward);
            }
        }

        private void BroadcastPlayerJumpEventKey()
        {
            bool isJumpKeyPressedNow = JumpKeys.Any(Input.IsKeyDown);

            if (isJumpKeyPressedNow && !_wasJumpKeyPressedLastFrame)
            {
                PlayerJumpEventKey.Broadcast(true);
            }
            _wasJumpKeyPressedLastFrame = isJumpKeyPressedNow;
        }
    }
}
