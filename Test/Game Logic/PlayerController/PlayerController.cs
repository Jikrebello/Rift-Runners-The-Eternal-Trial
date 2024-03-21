using Stride.Core.Diagnostics;
using Stride.Engine;
using Stride.Physics;
using Test.PlayerController.StateMachine.Combat;
using Test.PlayerController.StateMachine.Locomotion;

namespace Test.PlayerController.StateMachine
{
    public class PlayerController : SyncScript
    {
        private StateMachine _combatStateMachine;
        private StateMachine _locomotionStateMachine;
        private PlayerContext _playerContext;

        public override void Start()
        {
            _playerContext = new PlayerContext
            {
                ScriptComponent = this,
                Game = Game,
                DebugText = DebugText,
                Log = Log,
                Character = Entity.Get<CharacterComponent>(),
                PlayerInput = Entity.Get<PlayerInput>(),
                Input = Input,
                Model = Entity.GetChild(0)
            };

            _playerContext.Log.ActivateLog(LogMessageType.Info);

            _combatStateMachine = new StateMachine(_playerContext);
            _playerContext.CombatStateMachine = _combatStateMachine;
            _combatStateMachine.TransitionTo(new CombatState());

            _locomotionStateMachine = new StateMachine(_playerContext);
            _playerContext.LocomotionStateMachine = _locomotionStateMachine;
            _locomotionStateMachine.TransitionTo(new IdleState());
        }

        public override void Update()
        {
            _locomotionStateMachine.HandleInput();
            _locomotionStateMachine.Update();

            _combatStateMachine.HandleInput();
            _combatStateMachine.Update();
        }
    }
}
