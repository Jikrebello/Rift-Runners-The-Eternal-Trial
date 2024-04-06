using System.Collections.Generic;

namespace Test.PlayerController.StateMachine
{
    public interface IState
    {
        PlayerContext Context { get; set; }

        void Enter(Dictionary<string, object> parameters);
        void BroadcastAnimationState();
        void HandleInput();
        void Update();
        void Exit();
    }
}
