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
    public sealed class TechType_AllDrawer : PropertyDrawer
    {
        public static Game TargetGame = 0;

        private static readonly List<string> SubnauticaTechTypes = typeof(TechType_All).GetEnumNames()
            .Where(n => typeof(TechType_All).GetField(n).GetCustomAttribute<TechTypeFlagsAttribute>().flags.HasFlag(TechTypeFlagsAttribute.Flags.Subnautica)).ToList();

        private static readonly List<string> BelowZeroTechTypes = typeof(TechType_All).GetEnumNames()
            .Where(n => typeof(TechType_All).GetField(n).GetCustomAttribute<TechTypeFlagsAttribute>().flags.HasFlag(TechTypeFlagsAttribute.Flags.BelowZero)).ToList();

        public static bool IsValueAcceptable(string entry, string propertyPath)
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

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.enumValueIndex < 0 || property.enumValueIndex >= property.enumDisplayNames.Length)
            {
                property.enumValueIndex = 0;
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            int controlid = DrawerUtils.ControlId(property.propertyPath, position);

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, controlid, label);

            DrawDropdownButton(property, controlid, position);

            EditorGUI.EndProperty();
        }

        public static void DrawDropdownButton(SerializedProperty property, int controlid, Rect position)
        {
            Color oldColor = GUI.backgroundColor;
            if (!IsValueAcceptable(property.enumNames[property.enumValueIndex], property.propertyPath)) GUI.backgroundColor = Color.red;

            if (DropdownButton(controlid, position, new GUIContent(property.enumDisplayNames[property.enumValueIndex])))
            {
                SearchablePopup.Show(position, property.enumDisplayNames, property.enumNames, property.enumValueIndex, property.propertyPath, IsValueAcceptable, i =>
                {
                    property.enumValueIndex = i;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            GUI.backgroundColor = oldColor;
        }

        /// <summary>
        /// A custom button drawer that allows for a controlID so that we can
        /// sync the button ID and the label ID to allow for keyboard
        /// navigation like the built-in enum drawers.
        /// </summary>
        private static bool DropdownButton(int id, Rect position, GUIContent content)
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && current.button == 0)
                    {
                        Event.current.Use();
                        return true;
                    }

                    break;
                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == id && current.character == '\n')
                    {
                        Event.current.Use();
                        return true;
                    }

                    break;
                case EventType.Repaint:
                    EditorStyles.popup.Draw(position, content, id, false);
                    break;
            }

            return false;
        }
    }
}
