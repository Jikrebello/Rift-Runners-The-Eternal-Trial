using System;
using Stride.Animations;
using Stride.Core;
using Stride.Core.Collections;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Events;
using Test.Game_Logic.Player.PlayerController.StateMachines.States.Locomotion;

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

        [Display("Time Scale")]
        public double TimeFactor { get; set; } = 1;

        public double CurrentTime { get; set; }

        private IAnimationState _currentState;

        // --- States ---
        private WalkingState _walkingState;

        public override void Start()
        {
            base.Start();

            CheckAnimationComponents();

            AnimationComponent.BlendTreeBuilder = this;

            _walkingState = new WalkingState(this);

            ChangeState(_walkingState);
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
