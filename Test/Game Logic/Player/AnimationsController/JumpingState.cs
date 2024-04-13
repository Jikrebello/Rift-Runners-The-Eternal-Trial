using System;
using Stride.Animations;
using Stride.Core.Collections;
using Stride.Games;

namespace Test.Game_Logic.Player.AnimationsController
{
    public class JumpingState : IAnimationState
    {
        private enum JumpPhase
        {
            Jump,
            Airborne,
            Landing
        }

        private readonly AnimationController _controller;
        private AnimationClipEvaluator _evaluatorJump;
        private AnimationClipEvaluator _evaluatorAirborne;
        private AnimationClipEvaluator _evaluatorLanding;

        private JumpPhase _currentPhase;
        private double _currentTime;

        public JumpingState(AnimationController controller)
        {
            _controller = controller;

            CreateAnimationClipEvaluators();
        }

        public void Enter()
        {
            _currentPhase = JumpPhase.Jump;
            _currentTime = 0;
        }

        public void Update(GameTime drawTime)
        {
            switch (_currentPhase)
            {
                case JumpPhase.Jump:
                    UpdateJump(drawTime);
                    break;
                case JumpPhase.Airborne:
                    UpdateAirborne(drawTime);
                    break;
                case JumpPhase.Landing:
                    UpdateLanding(drawTime);
                    break;
            }
        }

        public void BuildBlendTree(FastList<AnimationOperation> blendStack, double currentTime)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
            switch (_currentPhase)
            {
                case JumpPhase.Jump:
                    blendStack.Add(AnimationOperation.NewPush(_evaluatorJump, timeSpan));
                    break;
                case JumpPhase.Airborne:
                    blendStack.Add(AnimationOperation.NewPush(_evaluatorAirborne, timeSpan));
                    break;
                case JumpPhase.Landing:
                    blendStack.Add(AnimationOperation.NewPush(_evaluatorLanding, timeSpan));
                    break;
            }
        }

        public void Exit()
        {
            _controller.AnimationComponent.Blender.ReleaseEvaluator(_evaluatorJump);
            _controller.AnimationComponent.Blender.ReleaseEvaluator(_evaluatorAirborne);
            _controller.AnimationComponent.Blender.ReleaseEvaluator(_evaluatorLanding);
        }

        private void CreateAnimationClipEvaluators()
        {
            _evaluatorJump = _controller.AnimationComponent.Blender.CreateEvaluator(
                _controller.AnimationJump
            );
            _evaluatorAirborne = _controller.AnimationComponent.Blender.CreateEvaluator(
                _controller.AnimationAirborne
            );
            _evaluatorLanding = _controller.AnimationComponent.Blender.CreateEvaluator(
                _controller.AnimationLanding
            );
        }

        private void UpdateJump(GameTime drawTime)
        {
            _currentTime += drawTime.Elapsed.TotalSeconds;
            double totalDuration = _controller.AnimationJump.Duration.TotalSeconds;

            if (_currentTime < totalDuration)
            {
                // Animation still playing
            }
            else
            {
                // Transition to next phase
                _currentPhase = JumpPhase.Airborne;
                _currentTime = 0;
            }
        }

        private void UpdateAirborne(GameTime drawTime)
        {
            _currentTime += drawTime.Elapsed.TotalSeconds;
            double totalDuration = _controller.AnimationAirborne.Duration.TotalSeconds;

            if (_currentTime < totalDuration)
            {
                // Animation still playing
                _currentTime %= totalDuration;
            }
            else
            {
                _currentPhase = JumpPhase.Landing;
                _currentTime = 0;
            }
        }

        private void UpdateLanding(GameTime drawTime)
        {
            _currentTime += drawTime.Elapsed.TotalSeconds;
            double totalDuration = _controller.AnimationLanding.Duration.TotalSeconds;

            if (_currentTime < totalDuration)
            {
                // Animation still playing
            }
            else
            {
                _controller.ChangeState(_controller.WalkingState); // Or transition to a new state
                _currentTime = 0; // Reset time for the next state or reinitialize jump
            }
        }
    }
}
