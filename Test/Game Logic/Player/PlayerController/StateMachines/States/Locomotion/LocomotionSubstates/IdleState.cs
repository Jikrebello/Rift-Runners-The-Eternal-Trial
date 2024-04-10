﻿using System.Collections.Generic;
using Stride.Core.Mathematics;
using Test.Game_Logic.Player.AnimationsController;

namespace Test.Game_Logic.Player.PlayerController.StateMachines.States.Locomotion.LocomotionSubstates
{
    public class IdleState : LocomotionState
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
            base.Update();

            Context.ScriptComponent.DebugText.Print("In Idle state", new Int2(350, 450));

            ShouldMoveToRunning(currentMoveDirection.LengthSquared() > float.Epsilon);

            HandleAiming();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void BroadcastAnimationState()
        {
            LocomotionAnimationStateEventKey.Broadcast(LocomotionAnimationState.Idle);
        }

        private void ShouldMoveToRunning(bool isntMoving)
        {
            if (isntMoving)
            {
                Context.LocomotionStateMachine.TransitionTo(
                    new RunningState(),
                    new Dictionary<string, object> { { "aiming", isAiming } }
                );
            }
        }

        private void HandleAiming()
        {
            if (isAiming)
            {
                Context.ScriptComponent.DebugText.Print("Strafe Idle", new Int2(350, 350));
            }
            else
            {
                Context.ScriptComponent.DebugText.Print("Normal Idle", new Int2(350, 350));
            }
        }
    }
}
