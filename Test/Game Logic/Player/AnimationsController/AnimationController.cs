using System;
using Stride.Animations;
using Stride.Core;
using Stride.Core.Collections;
using Stride.Engine;
using Stride.Engine.Events;

namespace Test.Game_Logic.Player.AnimationsController
{
    public class AnimationController : SyncScript, IBlendTreeBuilder
    {
        [Display("Animation Component")]
        public AnimationComponent AnimationComponent { get; set; }

        // --- Animation Clips ---
        [Display("Idle")]
        public AnimationClip AnimationClipIdle { get; set; }

        [Display("Walk")]
        public AnimationClip AnimationClipWalk { get; set; }

        [Display("Run")]
        public AnimationClip AnimationClipRun { get; set; }

        [Display("Jump")]
        public AnimationClip AnimationClipJump { get; set; }

        [Display("Mid-Air")]
        public AnimationClip AnimationClipMid_Air { get; set; }

        [Display("Landing")]
        public AnimationClip AnimationClipLanding { get; set; }

        [Display("Time Scale")]
        public double TimeFactor { get; set; } = 1;

        public double CurrentTime { get; set; }
        public bool IsGrounded { get; set; }

        // --- States ---
        public WalkingAnimationState WalkingState;
        public JumpAnimationState JumpAnimationState;
        public AirborneAnimationState AirborneAnimationState;
        public LandingAnimationState LandingAnimationState;

        private IAnimationState _currentState;

        //private EventReceiver<bool> _playerGroundedEventReceiver;

        public override void Start()
        {
            base.Start();

            CheckAnimationComponents();

            AnimationComponent.BlendTreeBuilder = this;

            WalkingState = new WalkingAnimationState(this);
            JumpAnimationState = new JumpAnimationState(this);
            AirborneAnimationState = new AirborneAnimationState(this);
            LandingAnimationState = new LandingAnimationState(this);

            //_playerGroundedEventReceiver = new EventReceiver<bool>()

            ChangeState(WalkingState);
        }

        public void ChangeState(IAnimationState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

        public override void Update()
        {
            _currentState?.Update(Game.DrawTime);
        }

        public void BuildBlendTree(FastList<AnimationOperation> animationList)
        {
            _currentState?.BuildBlendTree(animationList, CurrentTime);
        }

        private void CheckAnimationComponents()
        {
            if (AnimationComponent == null)
            {
                throw new InvalidOperationException("The animation component is not set.");
            }

            if (AnimationClipIdle == null)
                throw new InvalidOperationException("Idle animation is not set");

            if (AnimationClipWalk == null)
                throw new InvalidOperationException("Walking animation is not set");

            if (AnimationClipRun == null)
                throw new InvalidOperationException("Running animation is not set");
        }
    }
}
