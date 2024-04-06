using System.Collections.Generic;
using Stride.Core.Mathematics;
using Test.PlayerController.StateMachine.Combat;

namespace Test.Game_Logic.Player.PlayerController.StateMachine.Combat.CombatSubstates
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
            base.BroadcastAnimationState();
        }

        public override void Update()
        {
            base.Update();

            Context.ScriptComponent.DebugText.Print("In Melee state", new Int2(550, 450));

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
