﻿using Stride.Core.Mathematics;
using System.Collections.Generic;

namespace Test.PlayerController.StateMachine.Locomotion
{
    public class EnemyClimbingState : LocomotionState
    {
        public override void Enter(Dictionary<string, object> parameters)
        {
            // Logic here
        }

        public override void Update()
        {
            base.Update(); // Logic here
            Context.ScriptComponent.DebugText.Print("In Enemy Climbing state", new Int2(350, 450));
        }

        public override void Exit()
        {
            // Logic here
        }
    }
}
