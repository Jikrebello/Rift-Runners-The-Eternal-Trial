using System;
using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Physics;

namespace Test.PlayerController
{
    public class ThirdPersonCamera : SyncScript
    {
        public float DefaultDistance { get; set; } = 6f;

        public float MinimumDistance { get; set; } = 0.4f;

        public float ConeRadius { get; set; } = 1.25f;

        public bool InvertX { get; set; } = false;

        public float MinVerticalAngle { get; set; } = -20f;

        public float MaxVerticalAngle { get; set; } = 70f;

        public bool InvertY { get; set; } = false;

        public float RotationSpeed { get; set; } = 360f;

        public float VerticalSpeed { get; set; } = 65f;

        private Vector3 cameraRotationXYZ = new(-20, 45, 0);
        private Vector3 targetRotationXYZ = new(-20, 45, 0);
        private readonly EventReceiver<Vector2> cameraDirectionEvent =
            new(PlayerInput.CameraRotateEventKey);

        private List<HitResult> resultsOutput;
        private ConeColliderShape coneShape;

        public override void Start()
        {
            base.Start();

            coneShape = new ConeColliderShape(DefaultDistance, ConeRadius, ShapeOrientation.UpZ);
            resultsOutput = [];

            if (Entity.GetParent() == null)
                throw new ArgumentException(
                    "ThirdPersonCamera should be placed as a child entity of its target entity!"
                );
        }

        public override void Update()
        {
            UpdateCameraRaycast();

            UpdateCameraOrientation();
        }

        private void UpdateCameraRaycast()
        {
            var maxLength = DefaultDistance;
            var cameraVector = new Vector3(0, 0, DefaultDistance);
            Entity.GetParent().Transform.Rotation.Rotate(ref cameraVector);

            if (ConeRadius <= 0)
            {
                // If the cone radius
                var raycastStart = Entity.GetParent().Transform.WorldMatrix.TranslationVector;
                var hitResult = this.GetSimulation()
                    .Raycast(raycastStart, raycastStart + cameraVector);
                if (hitResult.Succeeded)
                {
                    maxLength = Math.Min(
                        DefaultDistance,
                        (raycastStart - hitResult.Point).Length()
                    );
                }
            }
            else
            {
                // If the cone radius is > 0 we will sweep an actual cone and see where it collides
                var fromMatrix =
                    Matrix.Translation(0, 0, -DefaultDistance * 0.5f)
                    * Entity.GetParent().Transform.WorldMatrix;
                var toMatrix =
                    Matrix.Translation(0, 0, DefaultDistance * 0.5f)
                    * Entity.GetParent().Transform.WorldMatrix;

                resultsOutput.Clear();
                var cfg = CollisionFilterGroups.DefaultFilter;
                var cfgf = CollisionFilterGroupFlags.DefaultFilter; // Intentionally ignoring the CollisionFilterGroupFlags.StaticFilter; to avoid collision with poles

                this.GetSimulation()
                    .ShapeSweepPenetrating(
                        coneShape,
                        fromMatrix,
                        toMatrix,
                        resultsOutput,
                        cfg,
                        cfgf
                    );

                foreach (var result in resultsOutput)
                {
                    if (result.Succeeded)
                    {
                        var signedVector =
                            result.Point
                            - Entity.GetParent().Transform.WorldMatrix.TranslationVector;
                        var signedDistance = Vector3.Dot(cameraVector, signedVector);

                        var currentLength = DefaultDistance * result.HitFraction;
                        if (signedDistance > 0 && currentLength < maxLength)
                            maxLength = currentLength;
                    }
                }
            }

            if (maxLength < MinimumDistance)
                maxLength = MinimumDistance;

            Entity.Transform.Position.Z = maxLength;
        }

        private void UpdateCameraOrientation()
        {
            // Camera movement from player input
            cameraDirectionEvent.TryReceive(out Vector2 cameraMovement);

            if (InvertY)
                cameraMovement.Y *= -1;
            targetRotationXYZ.X += cameraMovement.Y * VerticalSpeed;
            targetRotationXYZ.X = Math.Max(targetRotationXYZ.X, -MaxVerticalAngle);
            targetRotationXYZ.X = Math.Min(targetRotationXYZ.X, -MinVerticalAngle);

            if (InvertX)
                cameraMovement.X *= -1;
            targetRotationXYZ.Y -= cameraMovement.X * RotationSpeed;

            cameraRotationXYZ = Vector3.Lerp(cameraRotationXYZ, targetRotationXYZ, 0.25f);
            Entity.GetParent().Transform.RotationEulerXYZ = new Vector3(
                MathUtil.DegreesToRadians(cameraRotationXYZ.X),
                MathUtil.DegreesToRadians(cameraRotationXYZ.Y),
                0
            );
        }
    }
}
