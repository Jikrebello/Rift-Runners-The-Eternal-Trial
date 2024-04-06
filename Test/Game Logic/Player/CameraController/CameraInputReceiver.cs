using Stride.Core.Mathematics;
using Stride.Engine.Events;

namespace Test.Game_Logic.Player.CameraController
{
    public class CameraInputReceiver
    {
        private readonly EventReceiver<Vector2> _cameraDirectionEvent;
        private readonly EventReceiver<bool> _aimingEvent;

        public CameraInputReceiver(
            EventKey<Vector2> cameraRotateEventKey,
            EventKey<bool> aimingEventKey
        )
        {
            _cameraDirectionEvent = new EventReceiver<Vector2>(cameraRotateEventKey);
            _aimingEvent = new EventReceiver<bool>(aimingEventKey);
        }

        public bool TryReceiveCameraMovement(out Vector2 cameraMovement) =>
            _cameraDirectionEvent.TryReceive(out cameraMovement);

        public bool TryReceiveIsAiming()
        {
            _aimingEvent.TryReceive(out bool isAiming);
            return isAiming;
        }
    }
}
