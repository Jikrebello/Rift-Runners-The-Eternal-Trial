using Stride.Core.Mathematics;
using Stride.Engine.Events;

namespace Test.Game_Logic.Camera
{
    public class CameraInputReceiver
    {
        private readonly EventReceiver<Vector2> cameraDirectionEvent;
        private readonly EventReceiver<bool> aimingEvent;

        public CameraInputReceiver(
            EventKey<Vector2> cameraRotateEventKey,
            EventKey<bool> aimingEventKey
        )
        {
            cameraDirectionEvent = new EventReceiver<Vector2>(cameraRotateEventKey);
            aimingEvent = new EventReceiver<bool>(aimingEventKey);
        }

        public bool TryReceiveCameraMovement(out Vector2 cameraMovement) =>
            cameraDirectionEvent.TryReceive(out cameraMovement);

        public bool TryReceiveIsAiming()
        {
            aimingEvent.TryReceive(out bool isAiming);
            return isAiming;
        }
    }
}
