using System.Collections.Generic;
using Stride.Core.Mathematics;

namespace Test.Game_Logic.Player.PlayerController.StateMachines.States.Airborne.AirborneSubstates
{
    public class GroundedState : AirborneState
    {
        public override void Enter(Dictionary<string, object> parameters)
        {
            base.Enter(parameters);
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void Update()
        {
            base.Update(); // Logic here
            Context.DebugText.Print("In Grounded state", new Int2(350, 320));

            ShouldMoveToFalling();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void BroadcastAnimationState()
        {
            base.BroadcastAnimationState();
        }

        private void ShouldMoveToFalling()
        {
            if (!Context.Character.IsGrounded)
            {
                Context.AirborneStateMachine.TransitionTo(
                    new FallingState(),
                    new Dictionary<string, object> { { "jumpsRemaining", jumpsRemaining }, }
                );
            }
        }
    }
}
