using System;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace Test.Game_Logic.Camera
{
    public class CameraOrientation
    {
        private Vector3 _cameraRotationXYZ = new(0, 0, 0);
        private Vector3 _targetRotationXYZ = new(0, 0, 0);
        private readonly float _lerpFactor = 0.25f;

        public void UpdateTargetRotation(Vector2 cameraMovement, Entity cameraTarget)
        {
            ApplyInversion(ref cameraMovement);

            _targetRotationXYZ.X += cameraMovement.Y * CameraSettings.VerticalSpeed;
            _targetRotationXYZ.X = Math.Clamp(
                _targetRotationXYZ.X,
                CameraSettings.MinVerticalAngle,
                CameraSettings.MaxVerticalAngle
            );

            _targetRotationXYZ.Y -= cameraMovement.X * CameraSettings.RotationSpeed;

            SmoothlyUpdateCameraRotation();

            ApplyRotationToEntity(cameraTarget);
        }

        private static void ApplyInversion(ref Vector2 cameraMovement)
        {
            if (CameraSettings.InvertY)
                cameraMovement.Y *= -1;
            if (CameraSettings.InvertX)
                cameraMovement.X *= -1;
        }

        private void SmoothlyUpdateCameraRotation()
        {
            _cameraRotationXYZ = Vector3.Lerp(_cameraRotationXYZ, _targetRotationXYZ, _lerpFactor);
        }

        private void ApplyRotationToEntity(Entity cameraTarget)
        {
            var currentRotation = _cameraRotationXYZ;

            cameraTarget.Transform.RotationEulerXYZ = new Vector3(
                MathUtil.DegreesToRadians(currentRotation.X),
                MathUtil.DegreesToRadians(currentRotation.Y),
                0
            );
        }
    }
}
