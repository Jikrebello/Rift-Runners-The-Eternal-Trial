using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Engine.Events;
using System.Collections.Generic;

namespace Test.PlayerController
{
    public class PlayerInput : SyncScript
    {
        public static readonly EventKey<Vector3> InputDirectionEventKey = new();
        public static readonly EventKey<Vector2> CameraDirectionEventKey = new();

        public CameraComponent Camera { get; set; }
        public float MouseSensitivity { get; set; } = 1f;

        private readonly Queue<Vector2> mouseDeltaHistory = new Queue<Vector2>();

        public override void Update()
        {
            var inputDirection = GetInputDirection();

            if (Camera != null)
            {
                inputDirection = ConvertInputToWorldDirection(
                    inputDirection,
                    Camera,
                    Vector3.UnitY
                );
            }
            InputDirectionEventKey.Broadcast(inputDirection);

            var cameraDirection = GetCameraDirection();
            CameraDirectionEventKey.Broadcast(cameraDirection);
        }

        private Vector3 GetInputDirection()
        {
            var moveDirection = new Vector2();
            moveDirection += new Vector2(
                (Input.IsKeyDown(Keys.A) ? -1 : 0) + (Input.IsKeyDown(Keys.D) ? 1 : 0),
                (Input.IsKeyDown(Keys.W) ? 1 : 0) + (Input.IsKeyDown(Keys.S) ? -1 : 0)
            );

            return new Vector3(moveDirection.X, 0, moveDirection.Y);
        }

        private Vector2 GetCameraDirection()
        {
            var cameraDirection = new Vector2();

            // Handle mouse locking/unlocking
            HandleMouseLocking();

            if (Input.IsMousePositionLocked)
            {
                // Apply smoothing to mouse movement (implement a smoothing method as needed)
                var smoothedMouseDelta = SmoothMouseMovement(Input.MouseDelta);

                // Apply sensitivity and time scaling once
                cameraDirection +=
                    new Vector2(smoothedMouseDelta.X, -smoothedMouseDelta.Y) * MouseSensitivity;
            }

            return cameraDirection;
        }

        private void HandleMouseLocking()
        {
            if (Input.IsMouseButtonDown(MouseButton.Left))
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

        private Vector2 SmoothMouseMovement(Vector2 currentMouseDelta)
        {
            // Exponential smoothing factor; adjust as needed (between 0 and 1)
            float smoothingFactor = 0.5f;

            if (mouseDeltaHistory.Count == 0)
                mouseDeltaHistory.Enqueue(currentMouseDelta);

            Vector2 lastSmoothedDelta = mouseDeltaHistory.Peek();
            Vector2 smoothedDelta = Vector2.Lerp(
                lastSmoothedDelta,
                currentMouseDelta,
                smoothingFactor
            );

            mouseDeltaHistory.Clear();
            mouseDeltaHistory.Enqueue(smoothedDelta);

            return smoothedDelta;
        }

        private static Vector3 ConvertInputToWorldDirection(
            Vector3 inputDirection,
            CameraComponent camera,
            Vector3 upVector
        )
        {
            camera.Update();
            var inverseView = Matrix.Invert(camera.ViewMatrix);

            var forward = Vector3.Cross(upVector, inverseView.Right);
            forward.Normalize();

            var right = Vector3.Cross(forward, upVector);
            right.Normalize();

            var worldDirection = forward * inputDirection.Z + right * inputDirection.X;
            return worldDirection;
        }
    }
}
