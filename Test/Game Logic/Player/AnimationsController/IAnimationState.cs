using Stride.Animations;
using Stride.Core.Collections;
using Stride.Games;

namespace Test.Game_Logic.Player.AnimationsController
{
    public interface IAnimationState
    {
        void Enter();
        void Update(GameTime gameTime);
        void BuildBlendTree(FastList<AnimationOperation> blendStack, double currentTime);
        void Exit();
    }
}
