using System.Collections.Generic;
using Stride.Core.Mathematics;

namespace Test.Game_Logic.Player.PlayerController.StateMachines.States.Interaction
{
    public class InteractionState : IState
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

        public void BroadcastAnimationState()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Update()
        {
            // Logic here
            Context.ScriptComponent.DebugText.Print("In Interaction state", new Int2(350, 350));
        }

        public virtual void Exit()
        {
            // Logic here
        }
    }
}
