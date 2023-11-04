using Stride.Core.Mathematics;
using Stride.Physics;
using System.Collections.Generic;

namespace Test.PlayerController.StateMachine.Locomotion
{
    public class LocomotionState : IState
    {
        public PlayerContext Context { get; set; }
        internal CharacterComponent _character;

        public virtual void Enter(Dictionary<string, object> parameters)
        {
            // Logic here
            _character = Context.CharacterComponent;
        }

        public virtual void Update()
        {
            // Logic here
            Context.ScriptComponent.DebugText.Print("In Locomotion state", new Int2(350, 350));
        }

        public virtual void Exit()
        {
            // Logic here
        }
    }
}
