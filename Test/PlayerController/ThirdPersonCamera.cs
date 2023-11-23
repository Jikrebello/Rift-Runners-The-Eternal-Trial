using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Events;
using System;

namespace Test.PlayerController
{
    public class ThirdPersonCamera : SyncScript
    {
        public float MinVerticalAngle { get; set; } = -20f;

        public float MaxVerticalAngle { get; set; } = 70f;

        public float RotationSpeed { get; set; } = 360f;

        public float VerticalSpeed { get; set; } = 65f;

        private Vector3 cameraRotationXYZ = new Vector3(0, 0, 0);
        private Vector3 targetRotationXYZ = new Vector3(0, 0, 0);
        private readonly EventReceiver<Vector2> cameraDirectionEvent = new EventReceiver<Vector2>(
            PlayerInput.CameraDirectionEventKey
        );

        public override void Start()
        {
            base.Start();

            if (Entity.GetParent() == null)
                throw new ArgumentException(
                    "ThirdPersonCamera should be placed as a child entity of its target entity!"
                );
        }

        public override void Update()
        {
            // Camera movement from player input
            Vector2 cameraMovement;
            cameraDirectionEvent.TryReceive(out cameraMovement);

            // Invert and update target rotation on X axis (Vertical)
            targetRotationXYZ.X += cameraMovement.Y * VerticalSpeed; // Inversion applied here
            // Clamp the X-axis rotation to the defined min/max vertical angles
            targetRotationXYZ.X = Math.Max(
                MinVerticalAngle,
                Math.Min(MaxVerticalAngle, targetRotationXYZ.X)
            );

            // Update target rotation on Y axis (Horizontal)
            targetRotationXYZ.Y += cameraMovement.X * RotationSpeed;

            // Interpolate towards the target rotation
            cameraRotationXYZ = Vector3.Lerp(cameraRotationXYZ, targetRotationXYZ, 0.25f);

            // Apply the rotation to the parent entity
            Entity.GetParent().Transform.RotationEulerXYZ = new Vector3(
                MathUtil.DegreesToRadians(cameraRotationXYZ.X),
                MathUtil.DegreesToRadians(cameraRotationXYZ.Y),
                0
            );
        }
    }
}
