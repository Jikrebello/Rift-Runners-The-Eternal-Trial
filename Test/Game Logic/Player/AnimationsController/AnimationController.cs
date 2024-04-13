using System;
using Stride.Animations;
using Stride.Core;
using Stride.Core.Collections;
using Stride.Engine;

namespace Test.Game_Logic.Player.AnimationsController
{
    public class AnimationController : SyncScript, IBlendTreeBuilder
    {
        [Display("Animation Component")]
        public AnimationComponent AnimationComponent { get; set; }

        // --- Animation Clips ---
        [Display("Idle")]
        public AnimationClip AnimationIdle { get; set; }

        [Display("Walk")]
        public AnimationClip AnimationWalk { get; set; }

        [Display("Run")]
        public AnimationClip AnimationRun { get; set; }

        [Display("Jump")]
        public AnimationClip AnimationJump { get; set; }

        [Display("Airborne")]
        public AnimationClip AnimationAirborne { get; set; }

        [Display("Landing")]
        public AnimationClip AnimationLanding { get; set; }

        [Display("Time Scale")]
        public double TimeFactor { get; set; } = 1;

        public double CurrentTime { get; set; }

        private IAnimationState _currentState;

        // --- States ---
        public WalkingState WalkingState;
        public JumpingState JumpingState;

        public override void Start()
        {
            base.Start();

            CheckAnimationComponents();

            AnimationComponent.BlendTreeBuilder = this;

            WalkingState = new WalkingState(this);
            JumpingState = new JumpingState(this);

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

            if (AnimationIdle == null)
                throw new InvalidOperationException("Idle animation is not set");

            if (AnimationWalk == null)
                throw new InvalidOperationException("Walking animation is not set");

            if (AnimationRun == null)
                throw new InvalidOperationException("Running animation is not set");
        }
    }
}
