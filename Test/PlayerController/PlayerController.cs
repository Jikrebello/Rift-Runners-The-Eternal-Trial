using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;
using Test.PlayerController.StateMachine.Airborne;
using Test.PlayerController.StateMachine.Aquatic;
using Test.PlayerController.StateMachine.Combat;
using Test.PlayerController.StateMachine.Interaction;
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
                CharacterComponent = Entity.Get<CharacterComponent>(),
                ScriptComponent = this
            };

            _combatStateMachine = new StateMachine(_playerContext);
            _combatStateMachine.TransitionTo(new CombatState());

            _locomotionStateMachine = new StateMachine(_playerContext);
            _locomotionStateMachine.TransitionTo(new IdleState());
        }

        public override void Update()
        {
            _combatStateMachine.Update();
            _locomotionStateMachine.Update();
        }
    }
}
