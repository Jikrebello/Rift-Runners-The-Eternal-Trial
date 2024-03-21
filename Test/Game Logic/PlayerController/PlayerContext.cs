using Stride.Core.Diagnostics;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Games;
using Stride.Input;
using Stride.Physics;
using Stride.Profiling;
using Test.PlayerController;
using Test.PlayerController.StateMachine;

namespace Test
{
    public class PlayerContext
    {
        // Stride Specific
        public ScriptComponent ScriptComponent { get; set; }
        public IGame Game { get; set; }

        public DebugTextSystem DebugText { get; set; }

        public Logger Log { get; set; }
        public CharacterComponent Character { get; set; }
        public InputManager Input { get; set; }

        public Entity Model { get; set; }

        public PlayerInput PlayerInput { get; set; }

        // State Machines
        public StateMachine CombatStateMachine { get; set; }
        public StateMachine LocomotionStateMachine { get; set; }

        // Event Receivers
        public EventReceiver<bool> AimingReceiver { get; set; }
    }
}
