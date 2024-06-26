﻿using System.Collections.Generic;
using Stride.Core.Mathematics;

namespace Test.Game_Logic.Player.PlayerController.StateMachines.States.Airborne.AirborneSubstates
{
    public class FallingState : AirborneState
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
            Context.DebugText.Print("In Falling state", new Int2(350, 320));

            ShouldMoveToGrounded();
        }

        public override void Exit()
        {
            base.Exit();
        }

        private void ShouldMoveToGrounded()
        {
            if (Context.Character.IsGrounded)
            {
                Context.AirborneStateMachine.TransitionTo(
                    new GroundedState(),
                    new Dictionary<string, object> { { "jumpsRemaining", jumpsRemaining }, }
                );
            }
        }
    }
}
