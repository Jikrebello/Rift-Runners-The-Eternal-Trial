using System.Collections.Generic;

namespace Test.Game_Logic.Player.PlayerController.StateMachines
{
    public class StateMachine
    {
        private IState _currentState;
        private readonly PlayerContext _playerContext;

        public StateMachine(PlayerContext playerContext)
        {
            _playerContext = playerContext;
        }

        public void HandleInput()
        {
            _currentState?.HandleInput();
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
