using System.Collections.Generic;
using Stride.Engine.Events;

namespace Test.Game_Logic.Player.PlayerController.StateMachines.States.Airborne
{
    public class AirborneState : IState
    {
        public PlayerContext Context { get; set; }

        public static readonly EventKey<bool> PlayerFallingEventKey =
            new("Player Event", "Falling");

        protected bool isJumping;
        protected bool isGrounded;
        protected bool isAiming;
        protected int currentAirJumps = 0;
        protected float timeSinceLastJump = 0f;

        private EventReceiver<bool> _playerJumpReceiver;
        private EventReceiver<bool> _aimingReceiver;

        private readonly float _jumpHeight = 10f;
        private readonly int _maxAirJumps = 1;
        private readonly float _jumpCooldownDuration = 0.2f;

        private bool _jumpProcessed = false;
        private bool _wasNotGroundedLastFrame = false;

        public virtual void Enter(Dictionary<string, object> parameters)
        {
            Context.Character.JumpSpeed = _jumpHeight;

            _playerJumpReceiver = new EventReceiver<bool>(PlayerInput.PlayerJumpEventKey);
            _aimingReceiver = new EventReceiver<bool>(PlayerInput.AimingEventKey);

            if (parameters != null)
            {
                if (parameters.TryGetValue("currentAirJumps", out object currentAirJumpsValue))
                    currentAirJumps = (int)currentAirJumpsValue;

                if (parameters.TryGetValue("timeSinceLastJump", out object timeSinceLastJumpValue))
                    timeSinceLastJump = (float)timeSinceLastJumpValue;
            }
        }

        public virtual void HandleInput()
        {
            _playerJumpReceiver.TryReceive(out isJumping);

            _aimingReceiver.TryReceive(out isAiming);
        }

        public virtual void Update()
        {
            isGrounded = Context.Character.IsGrounded;
            bool currentlyGrounded = Context.Character.IsGrounded;

            UpdateCooldownTimer();

            ProcessLanding(currentlyGrounded);
            HandleJumping(currentlyGrounded);
            ResetJumpProcessedState();

            _wasNotGroundedLastFrame = !currentlyGrounded;
        }

        public virtual void Exit() { }

        public virtual void BroadcastAnimationState() { }

        private void UpdateCooldownTimer()
        {
            if (timeSinceLastJump < _jumpCooldownDuration)
            {
                timeSinceLastJump += (float)Context.DeltaTime;
            }
        }

        private void ProcessLanding(bool currentlyGrounded)
        {
            if (currentlyGrounded && _wasNotGroundedLastFrame)
            {
                currentAirJumps = 0;
                _jumpProcessed = false;
                timeSinceLastJump = _jumpCooldownDuration;
            }
            else if (!currentlyGrounded)
            {
                PlayerFallingEventKey.Broadcast(true);
            }
        }

        private void HandleJumping(bool currentlyGrounded)
        {
            bool jumpButtonPressed = isJumping && !_jumpProcessed;
            bool canJumpAgain = currentlyGrounded || CanPerformAirJump();

            if (jumpButtonPressed && canJumpAgain && IsJumpCooldownComplete())
            {
                PerformJump(currentlyGrounded);
            }
        }

        private bool CanPerformAirJump()
        {
            return currentAirJumps < _maxAirJumps;
        }

        private bool IsJumpCooldownComplete()
        {
            return timeSinceLastJump >= _jumpCooldownDuration;
        }

        private void PerformJump(bool currentlyGrounded)
        {
            Context.Character.Jump();
            _jumpProcessed = true;
            timeSinceLastJump = 0;

            if (!currentlyGrounded)
            {
                currentAirJumps++;
            }
        }

        private void ResetJumpProcessedState()
        {
            if (_jumpProcessed && !isJumping)
            {
                _jumpProcessed = false;
            }
        }
    }
}
