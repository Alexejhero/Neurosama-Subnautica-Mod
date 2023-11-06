using System;

namespace SCHIZO.Registering
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class GameAttribute(Game game) : Attribute
    {
        public readonly Game game = game;

        public GameAttribute() : this(GameX.Any) { }
    }

    [Flags]
    public enum Game
    {
        Subnautica = 1,
        BelowZero = 2,
    }

    public static class GameX
    {
        public const Game Any = 0;
        public const Game Both = Game.Subnautica | Game.BelowZero;
    }
}
