using System.Reflection;
using Editor.Scripts.Extensions;
using SCHIZO.Helpers;
using SCHIZO.Interop.Subnautica.Enums;
using SCHIZO.Registering;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.PropertyDrawers.Enums
{
    [CustomPropertyDrawer(typeof(TechType_All))]
    public sealed class TechType_AllDrawer : GameSpecificEnumDrawer<TechType_All>
    {
        public static Game TargetGame;

        protected override bool IsValueAcceptable(string entry, string propertyPath)
        {
            return TargetGame == default
                ? base.IsValueAcceptable(entry, propertyPath)
                : IsValueAcceptable(entry, TargetGame);
        }

        public static void DrawDropdownButtonStatic(SerializedProperty property, int controlid, Rect position)
        {
            TechType_AllDrawer drawer = new();
            ReflectionCache.GetField(typeof(PropertyDrawer), "m_FieldInfo")
                .SetValue(drawer, property.GetFieldInfoAndStaticType(out _));
            drawer.DrawDropdownButton(property, controlid, position);
        }
    }
}
