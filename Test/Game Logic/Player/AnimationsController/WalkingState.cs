using System;
using Stride.Animations;
using Stride.Core.Collections;
using Stride.Core.Mathematics;
using Stride.Engine.Events;
using Stride.Games;
using Test.Game_Logic.Player.PlayerController.StateMachines.States.Locomotion;

namespace Test.Game_Logic.Player.AnimationsController
{
    public class WalkingState : IAnimationState
    {
        private readonly AnimationController _controller;
        private AnimationClipEvaluator _evaluatorIdle;
        private AnimationClipEvaluator _evaluatorWalk;
        private AnimationClipEvaluator _evaluatorRun;

        private readonly float _walkThreshold = 0.25f;

        // --- Idle-Walk-Run Animation Flow ---
        private AnimationClipEvaluator _evaluatorForLowerSpeed;
        private AnimationClipEvaluator _evaluatorForHigherSpeed;
        private AnimationClip _clipForLowerSpeed;
        private AnimationClip _clipForHigherSpeed;
        private float _blendFactorBetweenSpeeds;

        private EventReceiver<float> _playerSpeedEventReceiver;

        private long _blendedMaxDuration;

        public WalkingState(AnimationController controller)
        {
            _controller = controller;

            CreateAnimationClipEvaluators();
        }

        public void Enter()
        {
            _playerSpeedEventReceiver = new EventReceiver<float>(
                LocomotionState.PlayerSpeedEventKey
            );

            _evaluatorForLowerSpeed = _evaluatorIdle;
            _evaluatorForHigherSpeed = _evaluatorWalk;

            _clipForLowerSpeed = _controller.AnimationIdle;
            _clipForHigherSpeed = _controller.AnimationWalk;
        }

        public void Update(GameTime gameTime)
        {
            // Calculate the running speed and update walkLerpFactor accordingly
            _playerSpeedEventReceiver.TryReceive(out float runSpeed);

            if (runSpeed < _walkThreshold)
            {
                _blendFactorBetweenSpeeds = runSpeed / _walkThreshold;
                _blendFactorBetweenSpeeds = (float)Math.Sqrt(_blendFactorBetweenSpeeds);

                _evaluatorForLowerSpeed = _evaluatorIdle;
                _evaluatorForHigherSpeed = _evaluatorWalk;

                _clipForLowerSpeed = _controller.AnimationIdle;
                _clipForHigherSpeed = _controller.AnimationWalk;
            }
            else
            {
                _blendFactorBetweenSpeeds = (runSpeed - _walkThreshold) / (1.0f - _walkThreshold);

                _evaluatorForLowerSpeed = _evaluatorWalk;
                _evaluatorForHigherSpeed = _evaluatorRun;

                _clipForLowerSpeed = _controller.AnimationWalk;
                _clipForHigherSpeed = _controller.AnimationRun;
            }

            // Update currentTime
            _blendedMaxDuration = (long)
                MathUtil.Lerp(
                    _clipForLowerSpeed.Duration.Ticks,
                    _clipForHigherSpeed.Duration.Ticks,
                    _blendFactorBetweenSpeeds
                );
            var currentTicks = TimeSpan.FromTicks(
                (long)(_controller.CurrentTime * _blendedMaxDuration)
            );
            currentTicks = TimeSpan.FromTicks(
                (currentTicks.Ticks + (long)(gameTime.Elapsed.Ticks * _controller.TimeFactor))
                    % _blendedMaxDuration
            );
            _controller.CurrentTime = (double)currentTicks.Ticks / _blendedMaxDuration;
        }

        public void BuildBlendTree(FastList<AnimationOperation> blendStack, double currentTime)
        {
            blendStack.Add(
                AnimationOperation.NewPush(
                    _evaluatorForLowerSpeed,
                    TimeSpan.FromTicks((long)(currentTime * _clipForLowerSpeed.Duration.Ticks))
                )
            );
            blendStack.Add(
                AnimationOperation.NewPush(
                    _evaluatorForHigherSpeed,
                    TimeSpan.FromTicks((long)(currentTime * _clipForHigherSpeed.Duration.Ticks))
                )
            );
            blendStack.Add(
                AnimationOperation.NewBlend(CoreAnimationOperation.Blend, _blendFactorBetweenSpeeds)
            );
        }

        public void Exit()
        {
            _controller.AnimationComponent.Blender.ReleaseEvaluator(_evaluatorIdle);
            _controller.AnimationComponent.Blender.ReleaseEvaluator(_evaluatorWalk);
            _controller.AnimationComponent.Blender.ReleaseEvaluator(_evaluatorRun);
        }

        private void CreateAnimationClipEvaluators()
        {
            _evaluatorIdle = _controller.AnimationComponent.Blender.CreateEvaluator(
                _controller.AnimationIdle
            );
            _evaluatorWalk = _controller.AnimationComponent.Blender.CreateEvaluator(
                _controller.AnimationWalk
            );
            _evaluatorRun = _controller.AnimationComponent.Blender.CreateEvaluator(
                _controller.AnimationRun
            );
        }
    }
}
