using System;
using Stride.Core.Mathematics;
using Stride.Engine;
using Test.Game_Logic.Player.PlayerController;

namespace Test.Game_Logic.Player.CameraController
{
    public class ThirdPersonCamera : SyncScript
    {
        private CameraOrientation _orientation;
        private CameraInputReceiver _inputReceiver;
        private Entity _cameraTarget;
        private CameraComponent _cameraComponent;
        private bool _isAiming = false;

        public override void Start()
        {
            base.Start();

            _orientation = new CameraOrientation();
            _inputReceiver = new CameraInputReceiver(
                PlayerInput.CameraRotateEventKey,
                PlayerInput.AimingEventKey
            );
            _cameraTarget = Entity.GetParent();
            _cameraComponent = Entity.Get<CameraComponent>();

            if (_cameraTarget == null)
            {
                throw new InvalidOperationException(
                    "ThirdPersonCamera must be attached to CameraTarget entity as a child."
                );
            }

            if (_cameraComponent == null)
            {
                throw new InvalidOperationException("No CameraComponent found on the entity.");
            }
        }

        public override void Update()
        {
            if (_inputReceiver.TryReceiveCameraMovement(out Vector2 cameraMovement))
            {
                _orientation.UpdateTargetRotation(cameraMovement, _cameraTarget);
            }

            _isAiming = _inputReceiver.TryReceiveIsAiming();
            UpdateCameraPositionAndFOV(_isAiming);
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

            _cameraComponent.VerticalFieldOfView = MathUtil.Lerp(
                _cameraComponent.VerticalFieldOfView,
                targetFOV,
                CameraSettings.TransitionSpeed * (float)Game.UpdateTime.WarpElapsed.TotalSeconds
            );
        }
    }
}
