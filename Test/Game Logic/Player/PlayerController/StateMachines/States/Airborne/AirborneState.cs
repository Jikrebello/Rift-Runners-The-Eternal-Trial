using System.Collections.Generic;
using Stride.Core.Mathematics;

namespace Test.Game_Logic.Player.PlayerController.StateMachines.States.Airborne
{
    public class AirborneState : IState
    {
        public PlayerContext Context { get; set; }

        public virtual void Enter(Dictionary<string, object> parameters)
        {
            // Logic here
        }

        public virtual void BroadcastAnimationState()
        {
            throw new System.NotImplementedException();
        }

        public virtual void HandleInput()
        {
            // Logic here
        }

        public virtual void Update()
        {
            // Logic here
            Context.ScriptComponent.DebugText.Print("In Airborne state", new Int2(350, 350));

            //if(!Context.Character.IsGrounded)
            //{

            //}
        }

        public virtual void Exit()
        {
            // Logic here
        }
    }
}
