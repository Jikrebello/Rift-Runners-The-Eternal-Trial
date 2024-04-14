using System.Collections.Generic;

namespace Test.Game_Logic.Player.PlayerController.StateMachines
{
    public interface IState
    {
        PlayerContext Context { get; set; }

        void Enter(Dictionary<string, object> parameters);
        void HandleInput();
        void Update();
        void Exit();
    }
}
