﻿using SCHIZO.Items.Data.Crafting;
using SCHIZO.Registering;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.PropertyDrawers.NonTriInspector.Objects
{
    [CustomPropertyDrawer(typeof(Recipe))]
    public sealed class RecipeDrawer : PropertyDrawer
    {
        private static bool IsOk(SerializedProperty property)
        {
            if (!property.objectReferenceValue) return true;

            Recipe recipe = (Recipe) property.objectReferenceValue;

            if (property.propertyPath.EndsWith("SN") && !recipe.game.HasFlag(Game.Subnautica)) return false;
            if (property.propertyPath.EndsWith("BZ") && !recipe.game.HasFlag(Game.BelowZero)) return false;
            return true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Color oldColor = GUI.backgroundColor;
            if (!IsOk(property)) GUI.backgroundColor = Color.red;

            EditorGUI.PropertyField(position, property, label);

            GUI.backgroundColor = oldColor;

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
