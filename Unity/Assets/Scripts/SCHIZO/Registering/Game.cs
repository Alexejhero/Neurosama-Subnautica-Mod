using System;

namespace SCHIZO.Registering
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class GameAttribute : Attribute
    {
        public readonly Game game;

        public GameAttribute(Game game)
        {
            this.game = game;
        }

        public GameAttribute() : this(Game.Any) { }
    }

    [Flags]
    public enum Game
    {
        Any = 0,
        Subnautica = 1,
        BelowZero = 2,
        Both = Subnautica | BelowZero,
    }
}
