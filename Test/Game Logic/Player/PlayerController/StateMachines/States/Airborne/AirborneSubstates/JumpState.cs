using System.Collections.Generic;
using Stride.Core.Mathematics;

namespace Test.Game_Logic.Player.PlayerController.StateMachines.States.Airborne.AirborneSubstates
{
    public class JumpState : AirborneState
    {
        public override void Enter(Dictionary<string, object> parameters)
        {
            // Logic here
        }

        public override void Update()
        {
            base.Update(); // Logic here
            Context.ScriptComponent.DebugText.Print("In Jump state", new Int2(350, 450));
        }

        public override void Exit()
        {
            // Logic here
        }
    }
}
