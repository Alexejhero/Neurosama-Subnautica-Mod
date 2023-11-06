using System;

namespace SCHIZO.Registering
{
    [AttributeUsage(AttributeTargets.Field)]
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
}
