using Stride.Core.Mathematics;
using System.Collections.Generic;

namespace Test.PlayerController.StateMachine.Locomotion
{
    public class SprintingState : LocomotionState
    {
        public override void Enter(Dictionary<string, object> parameters)
        {
            // Logic here
            base.Enter(parameters);
        }

        public override void Update()
        {
            base.Update(); // Logic here
            Context.ScriptComponent.DebugText.Print("In Sprinting state", new Int2(350, 450));
        }

        public override void Exit()
        {
            // Logic here
        }
    }
}
