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

        public override void HandleInput()
        {
            // Logic here
            if (!Context.Input.IsMouseButtonDown(Stride.Input.MouseButton.Right))
            {
                Context.OnAimEventKeyHandler(false);
                Context.CombatStateMachine.TransitionTo(new CombatState());
            }
        }

        public override void Update()
        {
            // Logic here
            //base.Update();
            Context.ScriptComponent.DebugText.Print("In Aiming state", new Int2(550, 450));
        }

        public override void Exit()
        {
            // Logic here
        }
    }
}
