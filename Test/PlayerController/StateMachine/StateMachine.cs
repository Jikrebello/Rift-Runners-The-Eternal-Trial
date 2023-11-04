using System.Collections.Generic;
using Stride.Engine;
using Stride.Physics;

namespace Test.PlayerController.StateMachine
{
    public class StateMachine
    {
        private IState _currentState;
        private PlayerContext _playerContext;

        public StateMachine(PlayerContext playerContext)
        {
            _playerContext = playerContext;
        }

        public void Update()
        {
            _currentState?.Update();
        }

        public void TransitionTo(
            IState newState,
            Dictionary<string, object> transitionParameters = null
        )
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Context = _playerContext;
            _currentState.Enter(transitionParameters);
        }
    }
}
