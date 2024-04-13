using Stride.Core.Mathematics;

namespace Test.Game_Logic.Player.PlayerController
{
    public class MovementManager
    {
        public Vector3 CurrentMoveDirection;
        public Vector3 NewMoveDirection;
        public Quaternion Orientation;

        public Vector3 CalculateCurrentMovementDirection(
            Vector3 currentMoveDirection,
            float VELOCITY_SMOOTH_FACTOR,
            float INPUT_RESPONSE_FACTOR
        )
        {
            return currentMoveDirection * VELOCITY_SMOOTH_FACTOR
                + NewMoveDirection * INPUT_RESPONSE_FACTOR;
        }

        public Quaternion HandleAimingOrientation(Vector3 cameraForward)
        {
            // Normalize and adjust camera forward direction if necessary
            cameraForward.Y = 0;
            cameraForward.Normalize();

            // Align the character's forward direction with the camera's
            Orientation = Quaternion.LookRotation(cameraForward, Vector3.UnitY);

            return Orientation;
        }

        public Vector3 CalculateRelativeMovementDirection(Quaternion newOrientation)
        {
            // Calculate relative movement direction
            var rotationMatrix = Matrix.RotationQuaternion(newOrientation);
            var relativeMovementDirection = Vector3.TransformNormal(
                NewMoveDirection,
                Matrix.Invert(rotationMatrix)
            );

            relativeMovementDirection.Normalize();

            return relativeMovementDirection;
        }
    }
}
