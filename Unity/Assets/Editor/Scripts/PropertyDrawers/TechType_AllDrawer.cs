using SCHIZO.Interop.Subnautica.Enums;
using SCHIZO.Registering;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(TechType_All))]
    public sealed class TechType_AllDrawer : GameSpecificEnumDrawer<TechType_All>
    {
        public static Game TargetGame = 0;

        protected override bool IsValueAcceptable(string entry, string propertyPath)
        {
            switch (TargetGame)
            {
                case 0:
                    return base.IsValueAcceptable(entry, propertyPath);

                case Game.Subnautica:
                    return SubnauticaValues.Contains(entry);

                case Game.BelowZero:
                    return BelowZeroValues.Contains(entry);

                case Game.Subnautica | Game.BelowZero:
                    return SubnauticaValues.Contains(entry) && BelowZeroValues.Contains(entry);

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
