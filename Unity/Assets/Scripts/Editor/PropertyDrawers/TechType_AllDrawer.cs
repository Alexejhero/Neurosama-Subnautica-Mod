using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SCHIZO.Interop.Subnautica.Enums;
using SCHIZO.Registering;
using UnityEditor;
using UnityEngine;

namespace PropertyDrawers
{
    [CustomPropertyDrawer(typeof(TechType_All))]
    public sealed class TechType_AllDrawer : GameSpecificEnumDrawer
    {
        public static Game TargetGame = 0;

        private static readonly List<string> SubnauticaTechTypes = typeof(TechType_All).GetEnumNames()
            .Where(n => typeof(TechType_All).GetField(n).GetCustomAttribute<GameAttribute>().game.HasFlag(Game.Subnautica)).ToList();

        private static readonly List<string> BelowZeroTechTypes = typeof(TechType_All).GetEnumNames()
            .Where(n => typeof(TechType_All).GetField(n).GetCustomAttribute<GameAttribute>().game.HasFlag(Game.BelowZero)).ToList();

        protected override bool IsValueAcceptable(string entry, string propertyPath)
        {
            switch (TargetGame)
            {
                case 0:
                    if (propertyPath.ToLower().Contains("sn")) return SubnauticaTechTypes.Contains(entry);
                    if (propertyPath.ToLower().Contains("bz")) return BelowZeroTechTypes.Contains(entry);
                    return SubnauticaTechTypes.Contains(entry) || BelowZeroTechTypes.Contains(entry);

                case Game.Subnautica:
                    return SubnauticaTechTypes.Contains(entry);

                case Game.BelowZero:
                    return BelowZeroTechTypes.Contains(entry);

                case Game.Subnautica | Game.BelowZero:
                    return SubnauticaTechTypes.Contains(entry) && BelowZeroTechTypes.Contains(entry);

                default:
                    return false;
            }
        }

        public static void DrawDropdownButtonStatic(SerializedProperty property, int controlid, Rect position)
        {
            new TechType_AllDrawer().DrawDropdownButton(property, controlid, position);
        }
    }
}
