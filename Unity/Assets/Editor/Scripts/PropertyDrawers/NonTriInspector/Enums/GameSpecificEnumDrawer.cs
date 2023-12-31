using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Editor.Scripts.Extensions;
using Editor.Scripts.PropertyDrawers.Utilities;
using SCHIZO.Registering;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.PropertyDrawers.NonTriInspector.Enums
{
    public abstract class GameSpecificEnumDrawer<T> : PropertyDrawer where T : Enum
    {
        protected static readonly List<string> SubnauticaValues = typeof(T).GetEnumNames()
            .Where(n => typeof(T).GetField(n).GetCustomAttribute<GameAttribute>().TryGetGame(out Game game) && game.HasFlag(Game.Subnautica))
            .ToList();

        protected static readonly List<string> BelowZeroValues = typeof(T).GetEnumNames()
            .Where(n => typeof(T).GetField(n).GetCustomAttribute<GameAttribute>().TryGetGame(out Game game) && game.HasFlag(Game.BelowZero)).ToList();

        protected bool? hasGameAttr;
        protected GameAttribute gameAttribute;
        protected SerializedProperty parentWithGameAttribute;
        protected virtual bool IsValueAcceptable(SerializedProperty property, string entry)
        {
            Game game = default;
            if (!hasGameAttr.HasValue)
            {
                hasGameAttr = property.TryGetAttributeInHierarchy(out gameAttribute, out parentWithGameAttribute);
            }

            gameAttribute?.TryGetGame(parentWithGameAttribute, out game);
            // todo replace with attributes to be explicit?
            if (property.propertyPath.EndsWith("SN")) game = Game.Subnautica;
            if (property.propertyPath.EndsWith("BZ")) game = Game.BelowZero;

            return IsValueAcceptable(entry, game);
        }

        protected bool IsValueAcceptable(string value, Game game)
        {
            return (!game.HasFlag(Game.Subnautica) || SubnauticaValues.Contains(value))
                && (!game.HasFlag(Game.BelowZero) || BelowZeroValues.Contains(value));
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

        protected void DrawDropdownButton(SerializedProperty property, int controlid, Rect position)
        {
            Color oldColor = GUI.backgroundColor;
            if (!IsValueAcceptable(property, property.enumNames[property.enumValueIndex])) GUI.backgroundColor = Color.red;

            if (DropdownButton(controlid, position, new GUIContent(property.enumDisplayNames[property.enumValueIndex])))
            {
                SearchablePopup.Show(position, property, IsValueAcceptable, i =>
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
