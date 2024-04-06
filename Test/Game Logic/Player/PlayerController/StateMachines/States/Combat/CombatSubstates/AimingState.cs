using System.Collections.Generic;
using Stride.Core.Mathematics;
using Test.Game_Logic.Player.AnimationController;

namespace Test.Game_Logic.Player.PlayerController.StateMachines.States.Combat.CombatSubstates
{
    public class AimingState : CombatState
    {
        public override void Enter(Dictionary<string, object> parameters)
        {
            // Logic here
            base.Enter(parameters);
        }

        public override void HandleInput()
        {
            // Logic here
            base.HandleInput();
        }

        public override void BroadcastAnimationState()
        {
            CombatAnimationStateEventKey.Broadcast(CombatAnimationState.Aiming);
        }

        public override void Update()
        {
            // Logic here
            base.Update();

            Context.ScriptComponent.DebugText.Print("In Aiming state", new Int2(550, 450));

            if (!isAiming)
            {
                Context.CombatStateMachine.TransitionTo(new MeleeState());
            }
        }

        public override void Exit()
        {
            // Logic here
        }
    }
}
