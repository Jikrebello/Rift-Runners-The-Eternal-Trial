using Stride.Core.Diagnostics;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Input;
using Stride.Physics;
using Test.PlayerController.StateMachine;

namespace Test
{
    public class PlayerContext
    {
        // Stride Specific
        public ScriptComponent ScriptComponent { get; set; }
        public Logger Log { get; set; }
        public CharacterComponent Character { get; set; }
        public InputManager Input { get; set; }

        public Entity Model { get; set; }

        // State Machines
        public StateMachine CombatStateMachine { get; set; }
        public StateMachine LocomotionStateMachine { get; set; }

        // Events
        public EventKey<bool> AimingEventKey = new("Player Event", "Aiming");
        public EventReceiver<bool> AimingReceiver { get; set; }

        public void OnAimEventKeyHandler(bool isAiming)
        {
            AimingEventKey.Broadcast(isAiming);
        }
    }
}
