using Stride.Core.Diagnostics;
using Stride.Engine;
using Stride.Physics;
using Stride.Profiling;
using Test.Game_Logic.Player.PlayerController.StateMachines;

namespace Test.Game_Logic.Player.PlayerController
{
    public class PlayerContext
    {
        // Stride Specific
        public ScriptComponent ScriptComponent { get; set; }
        public DebugTextSystem DebugText { get; set; }
        public Logger Log { get; set; }
        public CharacterComponent Character { get; set; }
        public Entity Model { get; set; }
        public double DeltaTime { get; set; }
        public double FixedDeltaTime { get; set; }

        // State Machines
        public StateMachine CombatStateMachine { get; set; }
        public StateMachine LocomotionStateMachine { get; set; }
        public StateMachine AirborneStateMachine { get; set; }
    }
}
