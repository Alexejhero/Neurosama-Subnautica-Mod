using System.Collections.Generic;
using SCHIZO.Items;
using SCHIZO.Utilities;
using UnityEditor;
using UnityEngine;

namespace PropertyDrawers
{
    [CustomPropertyDrawer(typeof(Recipe))]
    public sealed class RecipeDrawer : PropertyDrawer
    {
        public static List<int> SubnauticaRecipes = new List<int>();
        public static List<int> BelowZeroRecipes = new List<int>();

        private static bool IsOk(SerializedProperty property)
        {
            if (!property.objectReferenceValue) return true;

            int instanceId = property.objectReferenceInstanceIDValue;
            Recipe recipe = (Recipe) property.objectReferenceValue;

            if (SubnauticaRecipes.Contains(instanceId) && property.name.ToLower().Contains("sn") && !recipe.game.HasFlag(Game.Subnautica)) return false;
            if (BelowZeroRecipes.Contains(instanceId) && property.name.ToLower().Contains("bz") && !recipe.game.HasFlag(Game.BelowZero)) return false;
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
