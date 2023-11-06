using SCHIZO.Helpers;
using SCHIZO.Registering;
using UnityEditor;

namespace Editor.Scripts.Extensions
{
    public static class GameAttributeExtensions
    {
        public static bool TryGetGame(this GameAttribute attr, out Game game) => TryGetGame(attr, null, out game);
        public static bool TryGetGame(this GameAttribute attr, SerializedProperty property, out Game game)
        {
            game = attr.game;
            if (game > 0 || string.IsNullOrEmpty(attr.gameMember)) return true;
            if (property == null) return false;

            object target = property.GetParent().GetSerializedValue<object>();
            game = ReflectionHelpers.GetMemberValue<Game>(target, attr.gameMember);
            return true;
        }
    }
}