using System;
using Stride.Animations;
using Stride.Core.Collections;
using Stride.Games;

namespace Test.Game_Logic.Player.AnimationsController
{
    public class JumpAnimationState : IAnimationState
    {
        private AnimationController _controller;
        private AnimationClipEvaluator _evaluator;

        public JumpAnimationState(AnimationController controller)
        {
            _controller = controller;
        }

        public void Enter()
        {
            _controller.CurrentTime = 0;
            _evaluator = _controller.AnimationComponent.Blender.CreateEvaluator(
                _controller.AnimationClipJump
            );
        }

        public void Update(GameTime gameTime)
        {
            var updatedTicks =
                _controller.CurrentTime * _controller.AnimationClipJump.Duration.Ticks
                + (long)(gameTime.Elapsed.Ticks * _controller.TimeFactor);
            if (updatedTicks < _controller.AnimationClipJump.Duration.Ticks)
            {
                _controller.CurrentTime =
                    updatedTicks / _controller.AnimationClipJump.Duration.Ticks;
            }
            else
            {
                _controller.ChangeState(_controller.AirborneAnimationState);
            }
        }

        public void BuildBlendTree(FastList<AnimationOperation> blendStack, double currentTime)
        {
            blendStack.Add(
                AnimationOperation.NewPush(
                    _evaluator,
                    TimeSpan.FromTicks(
                        (long)(currentTime * _controller.AnimationClipJump.Duration.Ticks)
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
