using System.Collections.Generic;
using Stride.Engine.Events;

namespace Test.Game_Logic.Player.PlayerController.StateMachines.States.Airborne
{
    public class AirborneState : IState
    {
        public PlayerContext Context { get; set; }

        public static readonly EventKey<bool> PlayerGroundedEventKey =
            new("Player Event", "Grounded");

        protected bool isJumping;
        protected bool isAiming;
        protected int jumpsRemaining;

        private EventReceiver<bool> _playerJumpReceiver;
        private EventReceiver<bool> _aimingReceiver;

        private readonly float _jumpHeight = 10f;
        private readonly double _reactionTimeThreshold = 0.3;
        private readonly int _maxAirJumps = 1;
        private double _reactionTimeRemaining;

        public virtual void Enter(Dictionary<string, object> parameters)
        {
            Context.Character.JumpSpeed = _jumpHeight;
            _reactionTimeRemaining = _reactionTimeThreshold;
            jumpsRemaining = _maxAirJumps;

            _playerJumpReceiver = new EventReceiver<bool>(PlayerInput.PlayerJumpEventKey);
            _aimingReceiver = new EventReceiver<bool>(PlayerInput.AimingEventKey);

            if (parameters != null)
            {
                if (parameters.TryGetValue("airJumpsLeft", out object airJumpsLeftValue))
                    jumpsRemaining = (int)airJumpsLeftValue;
            }
        }

        public virtual void HandleInput()
        {
            _playerJumpReceiver.TryReceive(out isJumping);

            _aimingReceiver.TryReceive(out isAiming);
        }

        public virtual void Update()
        {
            UpdateReactionTime();
            BroadcastPlayerGroundState();
            HandleJumpLogic();
        }

        private void UpdateReactionTime()
        {
            if (_reactionTimeThreshold > 0)
            {
                _reactionTimeRemaining -= Context.FixedDeltaTime;
                if (Context.Character.IsGrounded || _reactionTimeRemaining <= 0)
                {
                    ResetReactionTime();
                }
            }
        }

        private void ResetReactionTime()
        {
            _reactionTimeRemaining = _reactionTimeThreshold;
            PlayerGroundedEventKey.Broadcast(Context.Character.IsGrounded);
        }

        private void BroadcastPlayerGroundState()
        {
            if (ShouldBroadcastGroundState())
            {
                PlayerGroundedEventKey.Broadcast(Context.Character.IsGrounded);
            }
        }

        private bool ShouldBroadcastGroundState()
        {
            return _reactionTimeThreshold <= 0
                    && !Context.Character.IsGrounded
                    && jumpsRemaining <= 0
                || !isJumping;
        }

        private void HandleJumpLogic()
        {
            if (isJumping && (Context.Character.IsGrounded || jumpsRemaining > 0))
            {
                ProcessJump();
            }
        }

        private void ProcessJump()
        {
            if (!Context.Character.IsGrounded)
            {
                jumpsRemaining--;
            }
            else
            {
                jumpsRemaining = _maxAirJumps;
            }

            Context.Character.Jump();
            _reactionTimeRemaining = 0;
            PlayerGroundedEventKey.Broadcast(false);
        }

        public virtual void Exit() { }
    }
}
