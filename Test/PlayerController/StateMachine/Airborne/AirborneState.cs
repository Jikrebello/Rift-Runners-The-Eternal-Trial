using System.Collections.Generic;
using Stride.Core.Mathematics;

namespace Test.PlayerController.StateMachine.Airborne
{
    public class AirborneState : IState
    {
        public PlayerContext Context { get; set; }

        public virtual void Enter(Dictionary<string, object> parameters)
        {
            // Logic here
        }

        public virtual void HandleInput()
        {
            // Logic here
        }

        public virtual void Update()
        {
            // Logic here
            Context.ScriptComponent.DebugText.Print("In Airborne state", new Int2(350, 350));
        }

        public virtual void Exit()
        {
            // Logic here
        }
    }
}
