using Editor.Scripts.Extensions;
using SCHIZO.Helpers;
using SCHIZO.Interop.Subnautica.Enums;
using SCHIZO.Registering;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.PropertyDrawers.NonTriInspector.Enums
{
    [CustomPropertyDrawer(typeof(TechType_All))]
    public sealed class TechType_AllDrawer : GameSpecificEnumDrawer<TechType_All>
    {
        public static Game TargetGame;

        protected override bool IsValueAcceptable(SerializedProperty property, string entry)
        {
            return TargetGame != default
                ? IsValueAcceptable(entry, TargetGame)
                : base.IsValueAcceptable(property, entry);
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
