using System;
using SCHIZO.Helpers;

namespace SCHIZO.Registering
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class GameAttribute : Attribute
    {
        private readonly Game _game;
        private readonly string _gameMember;

        public GameAttribute(Game game = default)
        {
            _game = game;
        }
        public GameAttribute(string memberName)
        {
            _gameMember = memberName;
        }

        public bool TryGetGame(out Game game) => TryGetGame(null, out game);
        public bool TryGetGame(object target, out Game game)
        {
            game = _game;
            if (game > 0 || string.IsNullOrEmpty(_gameMember)) return true;
            if (target == null) return false;
            game = ReflectionHelpers.GetMemberValue<Game>(target, _gameMember);
            return true;
        }
    }

    [Flags]
    public enum Game
    {
        Subnautica = 1,
        BelowZero = 2,
    }
}
