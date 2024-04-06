//using System;
//using System.Collections.Generic;
//using Stride.Core.Mathematics;
//using Stride.Engine;
//using Stride.Physics;

//namespace Test.Game_Logic.Camera
//{
//    public class ThirdPersonCamera : SyncScript
//    {
//        public float DefaultDistance { get; set; } = 6f;
//        public float MinimumDistance { get; set; } = 0.4f;
//        public float ConeRadius { get; set; } = 1.25f;
//        public bool InvertX { get; set; } = false;
//        public float MinVerticalAngle { get; set; } = -20f;
//        public float MaxVerticalAngle { get; set; } = 70f;
//        public bool InvertY { get; set; } = false;
//        public float RotationSpeed { get; set; } = 360f;
//        public float VerticalSpeed { get; set; } = 65f;

//        private Vector3 cameraRotationXYZ = new(-20, 45, 0);
//        private Vector3 targetRotationXYZ = new(-20, 45, 0);
//        private readonly EventReceiver<Vector2> cameraDirectionEvent =
//            new(PlayerInput.CameraRotateEventKey);

//        private List<HitResult> resultsOutput;
//        private ConeColliderShape coneShape;

//        public override void Start()
//        {
//            base.Start();

//            InitializeConeShape();
//            ValidateParentEntity();
//        }

//        public override void Update()
//        {
//            PerformCameraRaycast();
//            AdjustCameraOrientation();
//        }

//        private void InitializeConeShape()
//        {
//            coneShape = new ConeColliderShape(DefaultDistance, ConeRadius, ShapeOrientation.UpZ);
//            resultsOutput = new List<HitResult>();
//        }

//        private void ValidateParentEntity()
//        {
//            if (Entity.GetParent() == null)
//                throw new ArgumentException(
//                    "ThirdPersonCamera should be placed as a child entity of its target entity!"
//                );
//        }

//        private void PerformCameraRaycast()
//        {
//            var maxLength = DefaultDistance;
//            var cameraVector = CalculateCameraVector();

//            if (ConeRadius <= 0)
//                maxLength = PerformRaycastOnly(cameraVector, maxLength);
//            else
//                maxLength = PerformConeSweep(cameraVector, maxLength);

//            maxLength = Math.Max(maxLength, MinimumDistance);
//            Entity.Transform.Position.Z = maxLength;
//        }

//        private Vector3 CalculateCameraVector()
//        {
//            var cameraVector = new Vector3(0, 0, DefaultDistance);
//            Entity.GetParent().Transform.Rotation.Rotate(ref cameraVector);
//            return cameraVector;
//        }

//        private float PerformRaycastOnly(Vector3 cameraVector, float maxLength)
//        {
//            var raycastStart = Entity.GetParent().Transform.WorldMatrix.TranslationVector;
//            var hitResult = this.GetSimulation().Raycast(raycastStart, raycastStart + cameraVector);
//            if (hitResult.Succeeded)
//            {
//                maxLength = Math.Min(DefaultDistance, (raycastStart - hitResult.Point).Length());
//            }
//            return maxLength;
//        }

//        private float PerformConeSweep(Vector3 cameraVector, float maxLength)
//        {
//            var fromMatrix = CreateMatrixOffset(-DefaultDistance * 0.5f);
//            var toMatrix = CreateMatrixOffset(DefaultDistance * 0.5f);

//            resultsOutput.Clear();
//            SweepCone(fromMatrix, toMatrix, cameraVector, ref maxLength);
//            return maxLength;
//        }

//        private Matrix CreateMatrixOffset(float zOffset)
//        {
//            return Matrix.Translation(0, 0, zOffset) * Entity.GetParent().Transform.WorldMatrix;
//        }

//        private void SweepCone(
//            Matrix fromMatrix,
//            Matrix toMatrix,
//            Vector3 cameraVector,
//            ref float maxLength
//        )
//        {
//            var cfg = CollisionFilterGroups.DefaultFilter;
//            var cfgf = CollisionFilterGroupFlags.DefaultFilter;

//            this.GetSimulation()
//                .ShapeSweepPenetrating(coneShape, fromMatrix, toMatrix, resultsOutput, cfg, cfgf);

//            foreach (var result in resultsOutput)
//            {
//                if (result.Succeeded)
//                {
//                    UpdateMaxLengthBasedOnCollision(result, cameraVector, ref maxLength);
//                }
//            }
//        }

//        private void UpdateMaxLengthBasedOnCollision(
//            HitResult result,
//            Vector3 cameraVector,
//            ref float maxLength
//        )
//        {
//            var signedVector =
//                result.Point - Entity.GetParent().Transform.WorldMatrix.TranslationVector;
//            var signedDistance = Vector3.Dot(cameraVector, signedVector);
//            var currentLength = DefaultDistance * result.HitFraction;

//            if (signedDistance > 0 && currentLength < maxLength)
//                maxLength = currentLength;
//        }

//        private void AdjustCameraOrientation()
//        {
//            cameraDirectionEvent.TryReceive(out Vector2 cameraMovement);

//            ApplyInversion(ref cameraMovement);
//            UpdateTargetRotation(cameraMovement);
//            SmoothlyUpdateCameraRotation();
//            ApplyRotationToEntity();
//        }

//        private void ApplyInversion(ref Vector2 cameraMovement)
//        {
//            if (InvertY)
//                cameraMovement.Y *= -1;
//            if (InvertX)
//                cameraMovement.X *= -1;
//        }

//        private void UpdateTargetRotation(Vector2 cameraMovement)
//        {
//            targetRotationXYZ.X += cameraMovement.Y * VerticalSpeed;
//            targetRotationXYZ.X = Math.Max(targetRotationXYZ.X, -MaxVerticalAngle);
//            targetRotationXYZ.X = Math.Min(targetRotationXYZ.X, -MinVerticalAngle);

//            targetRotationXYZ.Y -= cameraMovement.X * RotationSpeed;
//        }

//        private void SmoothlyUpdateCameraRotation()
//        {
//            cameraRotationXYZ = Vector3.Lerp(cameraRotationXYZ, targetRotationXYZ, 0.25f);
//        }

//        private void ApplyRotationToEntity()
//        {
//            Entity.GetParent().Transform.RotationEulerXYZ = new Vector3(
//                MathUtil.DegreesToRadians(cameraRotationXYZ.X),
//                MathUtil.DegreesToRadians(cameraRotationXYZ.Y),
//                0
//            );
//        }
//    }



//    private void AdjustCameraPosition(float distance)
//    {
//        // Assumes the camera is looking towards the negative Z direction from the parent entity
//        var cameraPosition =
//            Entity.GetParent().Transform.Position
//            - Entity.Transform.WorldMatrix.Forward * distance;
//        Entity.Transform.Position = cameraPosition;
//    }

//    public class CameraCollision
//    {
//        private readonly List<HitResult> resultsOutput = [];
//        private readonly ConeColliderShape coneShape;
//        private readonly Simulation simulation;
//        private readonly Entity parentEntity;
//        private readonly float defaultDistance;
//        private readonly float minimumDistance;

//        public CameraCollision(
//            Simulation simulation,
//            Entity parentEntity,
//            float minimumDistance,
//            float defaultDistance,
//            float coneRadius
//        )
//        {
//            this.simulation = simulation;
//            this.parentEntity = parentEntity;
//            this.minimumDistance = minimumDistance;
//            this.defaultDistance = defaultDistance;
//            coneShape = new ConeColliderShape(defaultDistance, coneRadius, ShapeOrientation.UpZ);
//        }

//        public float PerformCameraRayCast()
//        {
//            var maxLength = defaultDistance;
//            var cameraVector = CalculateCameraVector();

//            if (coneShape.Radius <= 0)
//                maxLength = PerformRaycastOnly(cameraVector, maxLength);
//            else
//                maxLength = PerformConeSweep(cameraVector, maxLength);

//            return Math.Max(maxLength, minimumDistance);
//        }

//        private Vector3 CalculateCameraVector()
//        {
//            var cameraVector = new Vector3(0, 0, defaultDistance); // Assumes the camera looks towards the negative Z direction from the parent entity
//            parentEntity.Transform.Rotation.Rotate(ref cameraVector);
//            return cameraVector;
//        }

//        private float PerformRaycastOnly(Vector3 cameraVector, float maxLength)
//        {
//            var raycastStart = parentEntity.Transform.WorldMatrix.TranslationVector;
//            var hitResult = simulation.Raycast(raycastStart, raycastStart + cameraVector);
//            if (hitResult.Succeeded)
//            {
//                maxLength = Math.Min(defaultDistance, (raycastStart - hitResult.Point).Length());
//            }
//            return maxLength;
//        }

//        private float PerformConeSweep(Vector3 cameraVector, float maxLength)
//        {
//            var fromMatrix = CreateMatrixOffset(-defaultDistance * 0.5f);
//            var toMatrix = CreateMatrixOffset(defaultDistance * 0.5f);

//            resultsOutput.Clear();
//            SweepCone(fromMatrix, toMatrix, cameraVector, ref maxLength);

//            return maxLength;
//        }

//        private Matrix CreateMatrixOffset(float zOffset)
//        {
//            return Matrix.Translation(0, 0, zOffset) * parentEntity.Transform.WorldMatrix;
//        }

//        private void SweepCone(
//            Matrix fromMatrix,
//            Matrix toMatrix,
//            Vector3 cameraVector,
//            ref float maxLength
//        )
//        {
//            simulation.ShapeSweepPenetrating(
//                coneShape,
//                fromMatrix,
//                toMatrix,
//                resultsOutput,
//                CollisionFilterGroups.DefaultFilter,
//                CollisionFilterGroupFlags.DefaultFilter
//            );

//            foreach (var result in resultsOutput)
//            {
//                if (result.Succeeded)
//                {
//                    UpdateMaxLengthBasedOnCollision(result, cameraVector, ref maxLength);
//                }
//            }
//        }

//        private void UpdateMaxLengthBasedOnCollision(
//            HitResult result,
//            Vector3 cameraVector,
//            ref float maxLength
//        )
//        {
//            var signedVector = result.Point - parentEntity.Transform.WorldMatrix.TranslationVector;
//            var signedDistance = Vector3.Dot(cameraVector, signedVector);
//            var currentLength = defaultDistance * result.HitFraction;

//            if (signedDistance > 0 && currentLength < maxLength)
//                maxLength = currentLength;
//        }
//    }
//}
