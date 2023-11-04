using Stride.Engine;
using System.Collections.Generic;

namespace Test.PlayerController.StateMachine
{
    public interface IState
    {
        PlayerContext Context { get; set; }
        void Enter(Dictionary<string, object> parameters);
        void Update();
        void Exit();
    }
}
