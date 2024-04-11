using System.Collections.Generic;
using Stride.Core.Mathematics;
using Test.Game_Logic.Player.AnimationsController;

namespace Test.Game_Logic.Player.PlayerController.StateMachines.States.Combat.CombatSubstates
{
    public class MeleeState : CombatState
    {
        public override void Enter(Dictionary<string, object> parameters)
        {
            base.Enter(parameters);
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void BroadcastAnimationState()
        {
            CombatAnimationStateEventKey.Broadcast(CombatAnimationState.Melee);
        }

        public override void Update()
        {
            base.Update();

            Context.DebugText.Print("In Melee state", new Int2(550, 450));

            if (isAiming)
            {
                Context.CombatStateMachine.TransitionTo(new AimingState());
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
