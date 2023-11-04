using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace Test.PlayerController.StateMachine.Combat
{
    public class CombatState : IState
    {
        public PlayerContext Context { get; set; }

        public virtual void Enter(Dictionary<string, object> parameters)
        {
            // Logic here
        }

        public virtual void Update()
        {
            // Logic here
            Context.ScriptComponent.DebugText.Print("In Combat state", new Int2(550, 350));
        }

        public virtual void Exit()
        {
            // Logic here
        }
    }
}
