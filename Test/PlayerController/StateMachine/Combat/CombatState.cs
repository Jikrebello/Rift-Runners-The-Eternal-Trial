using System.Collections.Generic;

namespace Test.PlayerController.StateMachine.Combat
{
    public class CombatState : IState
    {
        public PlayerContext Context { get; set; }

        public virtual void Enter(Dictionary<string, object> parameters)
        {
            // Logic here
        }

        public virtual void HandleInput()
        {
            // Logic here
            if (Context.Input.IsMouseButtonDown(Stride.Input.MouseButton.Right))
            {
                Context.OnAimEventKeyHandler(true);
                Context.CombatStateMachine.TransitionTo(new AimingState());
            }
        }

        public virtual void Update()
        {
            // Logic here
            //Context.ScriptComponent.DebugText.Print("In Combat state", new Int2(550, 350));
        }

        public virtual void Exit()
        {
            // Logic here
        }
    }
}
