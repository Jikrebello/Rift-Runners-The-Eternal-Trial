using Stride.Core.Mathematics;
using System.Collections.Generic;

namespace Test.PlayerController.StateMachine.Combat
{
    public class AimingState : CombatState
    {
        public override void Enter(Dictionary<string, object> parameters)
        {
            // Logic here
        }

        public override void Update()
        {
            base.Update(); // Logic here
            Context.ScriptComponent.DebugText.Print("In Aiming state", new Int2(450, 450));
        }

        public override void Exit()
        {
            // Logic here
        }
    }
}
