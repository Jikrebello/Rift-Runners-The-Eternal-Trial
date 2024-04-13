using System;
using Stride.Animations;
using Stride.Core.Collections;
using Stride.Games;

namespace Test.Game_Logic.Player.AnimationsController
{
    public class AirborneAnimationState : IAnimationState
    {
        private AnimationController _controller;
        private AnimationClipEvaluator _evaluator;

        public AirborneAnimationState(AnimationController controller)
        {
            _controller = controller;
        }

        public void Enter()
        {
            _controller.CurrentTime = 0; // Reset the time when entering the state
            _evaluator = _controller.AnimationComponent.Blender.CreateEvaluator(
                _controller.AnimationClipMid_Air
            );
        }

        public void Update(GameTime gameTime)
        {
            var ticks = (long)(
                _controller.CurrentTime * _controller.AnimationClipMid_Air.Duration.Ticks
            );
            var updatedTicks = ticks + (long)(gameTime.Elapsed.Ticks * _controller.TimeFactor);
            _controller.CurrentTime =
                (updatedTicks % _controller.AnimationClipMid_Air.Duration.Ticks)
                / (double)_controller.AnimationClipMid_Air.Duration.Ticks;

            if (!_controller.IsGrounded)
            {
                _controller.ChangeState(_controller.LandingAnimationState); // Change to landing state if grounded
            }
        }

        public void BuildBlendTree(FastList<AnimationOperation> blendStack, double currentTime)
        {
            blendStack.Add(
                AnimationOperation.NewPush(
                    _evaluator,
                    TimeSpan.FromTicks(
                        (long)(currentTime * _controller.AnimationClipMid_Air.Duration.Ticks)
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
