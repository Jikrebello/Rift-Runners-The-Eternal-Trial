using System.Collections.Generic;
using Stride.Core.Mathematics;

namespace Test.Game_Logic.Player.PlayerController.StateMachines.States.Aquatic.AquaticSubstates
{
    public class UnderwaterSwimmingState : AquaticState
    {
        public override void Enter(Dictionary<string, object> parameters)
        {
            // Logic here
        }

        public override void Update()
        {
            base.Update(); // Logic here
            Context.ScriptComponent.DebugText.Print("In Underwater state", new Int2(350, 450));
        }

        public override void Exit()
        {
            // Logic here
        }
    }
}
