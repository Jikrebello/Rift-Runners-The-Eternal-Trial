using Stride.Animations;
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Collections;
using Stride.Engine;

namespace Test.Game_Logic.Player.AnimationController
{
    public class AnimationController : SyncScript, IBlendTreeBuilder
    {
        [Display("Animation Component")]
        public AnimationComponent AnimationComponent { get; set; }

        [Display("Idle")]
        public AnimationClip AnimationClipIdle { get; set; }
        private AnimationClipEvaluator _idleAnimationEvaluator;

        [Display("Walk")]
        public AnimationClip AnimationClipWalking { get; set; }
        private AnimationClipEvaluator _walkingAnimationEvaluator;

        [Display("Run")]
        public AnimationClip AnimationClipRunning { get; set; }
        private AnimationClipEvaluator _runningAnimationEvaluator;

        [DataMemberRange(0, 1, 0.01, 0.1, 3)]
        [Display("Walk Threshold")]
        public float WalkThreshold { get; set; } = 0.25f;

        [Display("Time Scale")]
        public double TimeFactor { get; set; } = 1;
        private double _currentTime = 0;

        public override void Start()
        {
            base.Start();

            AnimationComponent.BlendTreeBuilder = this;

            _idleAnimationEvaluator = AnimationComponent.Blender.CreateEvaluator(AnimationClipIdle);
            _walkingAnimationEvaluator = AnimationComponent.Blender.CreateEvaluator(
                AnimationClipWalking
            );
            _runningAnimationEvaluator = AnimationComponent.Blender.CreateEvaluator(
                AnimationClipRunning
            );
        }

        public override void Update() { }

        public void BuildBlendTree(FastList<AnimationOperation> animationList) { }

        public override void Cancel() { }
    }
}
