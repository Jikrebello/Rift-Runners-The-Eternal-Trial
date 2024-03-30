using System;
using Stride.Core.Mathematics;
using Stride.Engine;
using Test.Game_Logic.Camera;

namespace Test.PlayerController
{
    public class ThirdPersonCamera : SyncScript
    {
        private CameraOrientation orientation;
        private CameraInputReceiver inputReceiver;
        private Entity cameraTarget;
        private CameraComponent cameraComponent;

        public override void Start()
        {
            base.Start();

            orientation = new CameraOrientation();
            inputReceiver = new CameraInputReceiver(
                PlayerInput.CameraRotateEventKey,
                PlayerInput.AimingEventKey
            );
            cameraTarget = Entity.GetParent();
            cameraComponent = Entity.Get<CameraComponent>();

            if (cameraTarget == null)
            {
                throw new InvalidOperationException(
                    "ThirdPersonCamera must be attached to CameraTarget entity as a child."
                );
            }

            if (cameraComponent == null)
            {
                throw new InvalidOperationException("No CameraComponent found on the entity.");
            }
        }

        public override void Update()
        {
            if (inputReceiver.TryReceiveCameraMovement(out Vector2 cameraMovement))
            {
                orientation.UpdateTargetRotation(cameraMovement, cameraTarget);
            }

            bool isAiming = inputReceiver.TryReceiveIsAiming();
            UpdateCameraPositionAndFOV(isAiming);
        }

        private void UpdateCameraPositionAndFOV(bool isAiming)
        {
            Vector3 targetPosition = isAiming
                ? CameraSettings.AimingCameraOffset
                : CameraSettings.BaseCameraPosition;

            float targetFOV = isAiming ? CameraSettings.AimingFOV : CameraSettings.DefaultFOV;

            Entity.Transform.Position = Vector3.Lerp(
                Entity.Transform.Position,
                targetPosition,
                CameraSettings.TransitionSpeed * (float)Game.UpdateTime.WarpElapsed.TotalSeconds
            );

            cameraComponent.VerticalFieldOfView = MathUtil.Lerp(
                cameraComponent.VerticalFieldOfView,
                targetFOV,
                CameraSettings.TransitionSpeed * (float)Game.UpdateTime.WarpElapsed.TotalSeconds
            );
        }
    }
}
