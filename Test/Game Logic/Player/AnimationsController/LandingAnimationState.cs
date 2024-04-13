using System;
using Stride.Animations;
using Stride.Core.Collections;
using Stride.Games;

namespace Test.Game_Logic.Player.AnimationsController
{
    public class LandingAnimationState : IAnimationState
    {
        private AnimationController _controller;
        private AnimationClipEvaluator _evaluator;

        public LandingAnimationState(AnimationController controller)
        {
            _controller = controller;
        }

        public void Enter()
        {
            _controller.CurrentTime = 0; // Reset the time when entering the state
            _evaluator = _controller.AnimationComponent.Blender.CreateEvaluator(
                _controller.AnimationClipLanding
            );
        }

        public void Update(GameTime gameTime)
        {
            var updatedTicks =
                _controller.CurrentTime * _controller.AnimationClipLanding.Duration.Ticks
                + (long)(gameTime.Elapsed.Ticks * _controller.TimeFactor);
            if (updatedTicks < _controller.AnimationClipLanding.Duration.Ticks)
            {
                _controller.CurrentTime =
                    updatedTicks / _controller.AnimationClipLanding.Duration.Ticks;
            }
            else
            {
                _controller.ChangeState(_controller.WalkingState); // Change to walking state after landing
            }
        }

        public void BuildBlendTree(FastList<AnimationOperation> blendStack, double currentTime)
        {
            blendStack.Add(
                AnimationOperation.NewPush(
                    _evaluator,
                    TimeSpan.FromTicks(
                        (long)(currentTime * _controller.AnimationClipLanding.Duration.Ticks)
                    )
                )
            );
        }

        public void Exit()
        {
            _controller.AnimationComponent.Blender.ReleaseEvaluator(_evaluator);
        }
    }
}
