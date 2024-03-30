using System.Collections.Generic;
using Stride.Core.Mathematics;

namespace Test.PlayerController.StateMachine.Combat
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

            if (aimingReceiver.TryReceive(out bool aiming))
            {
                isAiming = aiming;
            }
        }

        public override void Update()
        {
            // Logic here
            base.Update();

            Context.ScriptComponent.DebugText.Print("In Aiming state", new Int2(550, 450));

            if (!isAiming)
            {
                Context.CombatStateMachine.TransitionTo(new CombatState());
            }
        }

        public override void Exit()
        {
            // Logic here
        }
    }
}
