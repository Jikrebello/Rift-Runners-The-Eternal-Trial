using System;
using Stride.Animations;
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Collections;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Events;
using Test.Game_Logic.Player.PlayerController.StateMachines.States.Airborne;
using Test.Game_Logic.Player.PlayerController.StateMachines.States.Locomotion;

namespace Test.Game_Logic.Player.AnimationsController
{
    public class AnimationController : SyncScript, IBlendTreeBuilder
    {
        [Display("Animation Component")]
        public AnimationComponent AnimationComponent { get; set; }

        [Display("Idle")]
        public AnimationClip AnimationIdle { get; set; }

        [Display("Walk")]
        public AnimationClip AnimationWalk { get; set; }

        [Display("Run")]
        public AnimationClip AnimationRun { get; set; }

        [Display("Jump")]
        public AnimationClip AnimationJumpStart { get; set; }

        [Display("Airborne")]
        public AnimationClip AnimationJumpMid { get; set; }

        [Display("Landing")]
        public AnimationClip AnimationJumpEnd { get; set; }

        [DataMemberRange(0, 1, 0.01, 0.1, 3)]
        [Display("Walk Threshold")]
        public float WalkThreshold { get; set; } = 0.25f;

        [Display("Time Scale")]
        public double TimeFactor { get; set; } = 1;

        private AnimationClipEvaluator _evaluatorIdle;
        private AnimationClipEvaluator _evaluatorWalk;
        private AnimationClipEvaluator _evaluatorRun;
        private AnimationClipEvaluator _evaluatorJumpStart;
        private AnimationClipEvaluator _evaluatorJumpMid;
        private AnimationClipEvaluator _evaluatorJumpEnd;
        private double _currentTime = 0;

        // Idle-Walk-Run lerp
        private AnimationClipEvaluator _evaluatorForLowerRunSpeed;
        private AnimationClipEvaluator _evaluatorForHigherRunSpeed;
        private AnimationClip _clipForLowerRunSpeed;
        private AnimationClip _clipForHigherRunSpeed;
        private float _blendFactorBetweenRunSpeeds = 0.5f;

        // Internal state
        private bool _isPlayerGrounded = false;
        private AnimationState _currentState = AnimationState.Airborne;
        private readonly EventReceiver<float> _playerRunSpeedEvent =
            new(LocomotionState.PlayerRunSpeedEventKey);
        private readonly EventReceiver<bool> _playerIsGroundedEvent =
            new(AirborneState.PlayerGroundedEventKey);

        float _playerRunSpeed;

        public override void Start()
        {
            base.Start();

            if (AnimationComponent == null)
                throw new InvalidOperationException("The animation component is not set");

            if (AnimationIdle == null)
                throw new InvalidOperationException("Idle animation is not set");

            if (AnimationWalk == null)
                throw new InvalidOperationException("Walking animation is not set");

            if (AnimationRun == null)
                throw new InvalidOperationException("Running animation is not set");

            if (AnimationJumpStart == null)
                throw new InvalidOperationException("Jumping animation is not set");

            if (AnimationJumpMid == null)
                throw new InvalidOperationException("Airborne animation is not set");

            if (AnimationJumpEnd == null)
                throw new InvalidOperationException("Landing animation is not set");

            // By setting a custom blend tree builder we can override the default behaviour of the animation system
            //  Instead, BuildBlendTree(FastList<AnimationOperation> blendStack) will be called each frame
            AnimationComponent.BlendTreeBuilder = this;

            _evaluatorIdle = AnimationComponent.Blender.CreateEvaluator(AnimationIdle);
            _evaluatorWalk = AnimationComponent.Blender.CreateEvaluator(AnimationWalk);
            _evaluatorRun = AnimationComponent.Blender.CreateEvaluator(AnimationRun);
            _evaluatorJumpStart = AnimationComponent.Blender.CreateEvaluator(AnimationJumpStart);
            _evaluatorJumpMid = AnimationComponent.Blender.CreateEvaluator(AnimationJumpMid);
            _evaluatorJumpEnd = AnimationComponent.Blender.CreateEvaluator(AnimationJumpEnd);

            // Initial walk lerp
            _blendFactorBetweenRunSpeeds = 0;
            _evaluatorForLowerRunSpeed = _evaluatorIdle;
            _evaluatorForHigherRunSpeed = _evaluatorWalk;
            _clipForLowerRunSpeed = AnimationIdle;
            _clipForHigherRunSpeed = AnimationWalk;
        }

        public override void Cancel()
        {
            AnimationComponent.Blender.ReleaseEvaluator(_evaluatorIdle);
            AnimationComponent.Blender.ReleaseEvaluator(_evaluatorWalk);
            AnimationComponent.Blender.ReleaseEvaluator(_evaluatorRun);
            AnimationComponent.Blender.ReleaseEvaluator(_evaluatorJumpStart);
            AnimationComponent.Blender.ReleaseEvaluator(_evaluatorJumpMid);
            AnimationComponent.Blender.ReleaseEvaluator(_evaluatorJumpEnd);
        }

        private void UpdateWalking()
        {
            if (_playerRunSpeed < WalkThreshold)
            {
                _blendFactorBetweenRunSpeeds = _playerRunSpeed / WalkThreshold;
                _blendFactorBetweenRunSpeeds = (float)Math.Sqrt(_blendFactorBetweenRunSpeeds); // Idle-Walk blend looks really weird, so skew the factor towards walking
                _evaluatorForLowerRunSpeed = _evaluatorIdle;
                _evaluatorForHigherRunSpeed = _evaluatorWalk;
                _clipForLowerRunSpeed = AnimationIdle;
                _clipForHigherRunSpeed = AnimationWalk;
            }
            else
            {
                _blendFactorBetweenRunSpeeds =
                    (_playerRunSpeed - WalkThreshold) / (1.0f - WalkThreshold);
                _evaluatorForLowerRunSpeed = _evaluatorWalk;
                _evaluatorForHigherRunSpeed = _evaluatorRun;
                _clipForLowerRunSpeed = AnimationWalk;
                _clipForHigherRunSpeed = AnimationRun;
            }

            // Use DrawTime rather than UpdateTime
            var time = Game.DrawTime;
            // This update function will account for animation with different durations, keeping a current time relative to the blended maximum duration
            long blendedMaxDuration = (long)
                MathUtil.Lerp(
                    _clipForLowerRunSpeed.Duration.Ticks,
                    _clipForHigherRunSpeed.Duration.Ticks,
                    _blendFactorBetweenRunSpeeds
                );

            var currentTicks = TimeSpan.FromTicks((long)(_currentTime * blendedMaxDuration));

            currentTicks =
                blendedMaxDuration == 0
                    ? TimeSpan.Zero
                    : TimeSpan.FromTicks(
                        (currentTicks.Ticks + (long)(time.Elapsed.Ticks * TimeFactor))
                            % blendedMaxDuration
                    );

            _currentTime = (double)currentTicks.Ticks / blendedMaxDuration;
        }

        private void UpdateJumping()
        {
            var speedFactor = 1;
            var currentTicks = TimeSpan.FromTicks(
                (long)(_currentTime * AnimationJumpStart.Duration.Ticks)
            );
            var updatedTicks =
                currentTicks.Ticks + (long)(Game.DrawTime.Elapsed.Ticks * TimeFactor * speedFactor);

            if (updatedTicks < AnimationJumpStart.Duration.Ticks)
            {
                currentTicks = TimeSpan.FromTicks(updatedTicks);
                _currentTime = (double)currentTicks.Ticks / AnimationJumpStart.Duration.Ticks;
            }
            else
            {
                _currentState = AnimationState.Airborne;
                _currentTime = 0;
                UpdateAirborne();
            }
        }

        private void UpdateAirborne()
        {
            // Use DrawTime rather than UpdateTime
            var time = Game.DrawTime;
            var currentTicks = TimeSpan.FromTicks(
                (long)(_currentTime * AnimationJumpMid.Duration.Ticks)
            );
            currentTicks = TimeSpan.FromTicks(
                (currentTicks.Ticks + (long)(time.Elapsed.Ticks * TimeFactor))
                    % AnimationJumpMid.Duration.Ticks
            );
            _currentTime = (double)currentTicks.Ticks / AnimationJumpMid.Duration.Ticks;
        }

        private void UpdateLanding()
        {
            var speedFactor = 1;
            var currentTicks = TimeSpan.FromTicks(
                (long)(_currentTime * AnimationJumpEnd.Duration.Ticks)
            );
            var updatedTicks =
                currentTicks.Ticks + (long)(Game.DrawTime.Elapsed.Ticks * TimeFactor * speedFactor);

            if (updatedTicks < AnimationJumpEnd.Duration.Ticks)
            {
                currentTicks = TimeSpan.FromTicks(updatedTicks);
                _currentTime = (double)currentTicks.Ticks / AnimationJumpEnd.Duration.Ticks;
            }
            else
            {
                _currentState = AnimationState.Walking;
                _currentTime = 0;
                UpdateWalking();
            }
        }

        public override void Update()
        {
            // State control
            _playerRunSpeedEvent.TryReceive(out _playerRunSpeed);
            _playerIsGroundedEvent.TryReceive(out bool isGroundedNewValue);
            if (_isPlayerGrounded != isGroundedNewValue)
            {
                _currentTime = 0;
                _isPlayerGrounded = isGroundedNewValue;
                _currentState =
                    (_isPlayerGrounded) ? AnimationState.Landing : AnimationState.Jumping;
            }

            switch (_currentState)
            {
                case AnimationState.Walking:
                    UpdateWalking();
                    break;
                case AnimationState.Jumping:
                    UpdateJumping();
                    break;
                case AnimationState.Airborne:
                    UpdateAirborne();
                    break;
                case AnimationState.Landing:
                    UpdateLanding();
                    break;
            }
        }

        /// <summary>
        /// BuildBlendTree is called every frame from the animation system when the <see cref="AnimationComponent"/> needs to be evaluated
        /// It overrides the default behaviour of the <see cref="AnimationComponent"/> by setting a custom blend tree
        /// </summary>
        /// <param name="blendStack">The stack of animation operations to be blended</param>
        public void BuildBlendTree(FastList<AnimationOperation> blendStack)
        {
            switch (_currentState)
            {
                case AnimationState.Walking:

                    {
                        // Note! The tree is laid out as a stack and has to be flattened before returning it to the animation system!
                        blendStack.Add(
                            AnimationOperation.NewPush(
                                _evaluatorForLowerRunSpeed,
                                TimeSpan.FromTicks(
                                    (long)(_currentTime * _clipForLowerRunSpeed.Duration.Ticks)
                                )
                            )
                        );
                        blendStack.Add(
                            AnimationOperation.NewPush(
                                _evaluatorForHigherRunSpeed,
                                TimeSpan.FromTicks(
                                    (long)(_currentTime * _clipForHigherRunSpeed.Duration.Ticks)
                                )
                            )
                        );
                        blendStack.Add(
                            AnimationOperation.NewBlend(
                                CoreAnimationOperation.Blend,
                                _blendFactorBetweenRunSpeeds
                            )
                        );
                    }
                    break;

                case AnimationState.Jumping:

                    {
                        blendStack.Add(
                            AnimationOperation.NewPush(
                                _evaluatorJumpStart,
                                TimeSpan.FromTicks(
                                    (long)(_currentTime * AnimationJumpStart.Duration.Ticks)
                                )
                            )
                        );
                    }
                    break;

                case AnimationState.Airborne:

                    {
                        blendStack.Add(
                            AnimationOperation.NewPush(
                                _evaluatorJumpMid,
                                TimeSpan.FromTicks(
                                    (long)(_currentTime * AnimationJumpMid.Duration.Ticks)
                                )
                            )
                        );
                    }
                    break;

                case AnimationState.Landing:

                    {
                        blendStack.Add(
                            AnimationOperation.NewPush(
                                _evaluatorJumpEnd,
                                TimeSpan.FromTicks(
                                    (long)(_currentTime * AnimationJumpEnd.Duration.Ticks)
                                )
                            )
                        );
                    }
                    break;
            }
        }

        enum AnimationState
        {
            Walking,
            Jumping,
            Airborne,
            Landing,
        }
    }
}
