using Stride.Engine;
using Stride.Physics;

namespace Test.PlayerController
{
    public class PlayerContext
    {
        public ScriptComponent ScriptComponent { get; set; }
        public CharacterComponent CharacterComponent { get; set; }
        // Add more shared resources if needed
    }
}
