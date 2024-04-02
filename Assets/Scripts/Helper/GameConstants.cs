using System.Numerics;

public static class GameConstants
{
    public static class Player
    {
        public const float HumanReactionMin = 0.50f;
        public const float HumanReactionMax = 0.09f;
    }
    public static class World
    {
        public static Vector2 DefaultLocation = new Vector2(-50, 0);
        public const float CameraAspecRatio = 1.7777778f;
        public const int MaxDifficulty = 3;
        public const int MinDifficulty = 1;
        public const int MinDifficulty_Test = 0;
        public const int MaxDifficulty_Test = 4;
    }
}
