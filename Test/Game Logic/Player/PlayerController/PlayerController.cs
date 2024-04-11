using Stride.Core.Diagnostics;
using Stride.Engine;
using Stride.Physics;
using Test.Game_Logic.Player.PlayerController.StateMachines;
using Test.Game_Logic.Player.PlayerController.StateMachines.States.Airborne.AirborneSubstates;
using Test.Game_Logic.Player.PlayerController.StateMachines.States.Combat.CombatSubstates;
using Test.Game_Logic.Player.PlayerController.StateMachines.States.Locomotion.LocomotionSubstates;

namespace Test.Game_Logic.Player.PlayerController
{
    public class PlayerController : SyncScript
    {
        private PlayerContext _playerContext;

        private StateMachine _airborneStateMachine;
        private StateMachine _combatStateMachine;
        private StateMachine _locomotionStateMachine;

        public override void Start()
        {
            SetupPlayerContext();

            InitializeStateMachines();
        }

        public override void Update()
        {
            ProcessStateMachines();

            _playerContext.DeltaTime = Game.UpdateTime.WarpElapsed.TotalSeconds;
        }

        private void SetupPlayerContext()
        {
            _playerContext = new PlayerContext
            {
                ScriptComponent = this,
                DebugText = DebugText,
                Log = Log,
                Character = Entity.Get<CharacterComponent>(),
                Model = Entity.GetChild(0),
            };
            _playerContext.Log.ActivateLog(LogMessageType.Info);
        }

        private void InitializeStateMachines()
        {
            // --- Airborne State Machine ---
            _airborneStateMachine = new StateMachine(_playerContext);
            _playerContext.AirborneStateMachine = _airborneStateMachine;
            _airborneStateMachine.TransitionTo(new GroundedState());

            // --- Combat State Machine ---
            _combatStateMachine = new StateMachine(_playerContext);
            _playerContext.CombatStateMachine = _combatStateMachine;
            _combatStateMachine.TransitionTo(new MeleeState());

            // --- Locomotion State Machine ---
            _locomotionStateMachine = new StateMachine(_playerContext);
            _playerContext.LocomotionStateMachine = _locomotionStateMachine;
            _locomotionStateMachine.TransitionTo(new IdleState());
        }

        private void ProcessStateMachines()
        {
            _airborneStateMachine.HandleInput();
            _airborneStateMachine.Update();

            _combatStateMachine.HandleInput();
            _combatStateMachine.Update();

            _locomotionStateMachine.HandleInput();
            _locomotionStateMachine.Update();
        }
    }
}
