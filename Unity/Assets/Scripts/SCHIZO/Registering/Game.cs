using System;

namespace SCHIZO.Registering
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class GameAttribute : Attribute
    {
        internal readonly Game game;
        internal readonly string gameMember;

        public GameAttribute(Game game = default)
        {
            this.game = game;
        }
        public GameAttribute(string memberName)
        {
            gameMember = memberName;
        }
    }

    [Flags]
    public enum Game
    {
        Subnautica = 1,
        BelowZero = 2,
    }
}
