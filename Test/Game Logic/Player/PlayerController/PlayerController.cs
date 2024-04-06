using Stride.Core.Diagnostics;
using Stride.Engine;
using Stride.Physics;
using Test.Game_Logic.Player.PlayerController.StateMachines;
using Test.Game_Logic.Player.PlayerController.StateMachines.States.Combat.CombatSubstates;
using Test.Game_Logic.Player.PlayerController.StateMachines.States.Locomotion.LocomotionSubstates;

namespace Test.Game_Logic.Player.PlayerController
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
                Model = Entity.GetChild(0),
            };
            _playerContext.Log.ActivateLog(LogMessageType.Info);

            // Initialize the State Machines
            _combatStateMachine = new StateMachine(_playerContext);
            _playerContext.CombatStateMachine = _combatStateMachine;
            _combatStateMachine.TransitionTo(new MeleeState());

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
