using Stride.Engine.Events;
using Stride.Engine;
using Stride.Input;
using System.Collections.Generic;
using Stride.Core.Mathematics;
using System.Linq;


namespace Test.PlayerController
{
    public class PlayerInput : SyncScript
    {
        public static readonly EventKey<Vector3> MovementEventKey = new();
        public static readonly EventKey<Vector2> CameraRotateEventKey = new();

        public CameraComponent PlayerCamera { get; set; }
        public float MouseSensitivity = 1f;

        public List<Keys> MoveLeftKeys { get; } = [];
        public List<Keys> MoveRightKeys { get; } = [];
        public List<Keys> MoveUpKeys { get; } = [];
        public List<Keys> MoveDownKeys { get; } = [];

        public override void Start()
        {
            base.Start();

            // Assigning WASD keys to the respective lists
            MoveLeftKeys.Add(Keys.A);
            MoveRightKeys.Add(Keys.D);
            MoveUpKeys.Add(Keys.W);
            MoveDownKeys.Add(Keys.S);
        }

        public override void Update()
        {
            Vector2 movementInput = Vector2.Zero;

            // Handle keyboard inputs for movement
            if (MoveLeftKeys.Any(key => Input.IsKeyDown(key)))
                movementInput += -Vector2.UnitX;
            if (MoveRightKeys.Any(key => Input.IsKeyDown(key)))
                movementInput += Vector2.UnitX;
            if (MoveUpKeys.Any(key => Input.IsKeyDown(key)))
                movementInput += Vector2.UnitY;
            if (MoveDownKeys.Any(key => Input.IsKeyDown(key)))
                movementInput += -Vector2.UnitY;

            // Convert movement input to world-space direction
            Vector3 worldMovementDirection =
                (PlayerCamera != null)
                    ? LogicDirectionToWorldDirection(movementInput, PlayerCamera, Vector3.UnitY)
                    : new Vector3(movementInput.X, 0, movementInput.Y);

            // Broadcast the movement direction
            MovementEventKey.Broadcast(worldMovementDirection);

            // Handle camera rotation with mouse input
            Vector2 cameraRotationInput =
                new Vector2(Input.MouseDelta.X, -Input.MouseDelta.Y) * MouseSensitivity;

            // Broadcast the camera rotation input
            CameraRotateEventKey.Broadcast(cameraRotationInput);

            // Handle cursor locking
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

        public static Vector3 LogicDirectionToWorldDirection(
            Vector2 logicDirection,
            CameraComponent camera,
            Vector3 upVector
        )
        {
            camera.Update();
            var inverseView = Matrix.Invert(camera.ViewMatrix);

            var forward = Vector3.Cross(upVector, inverseView.Right);
            forward.Normalize();

            var right = Vector3.Cross(forward, upVector);
            var worldDirection = forward * logicDirection.Y + right * logicDirection.X;
            worldDirection.Normalize();
            return worldDirection;
        }
    }
}