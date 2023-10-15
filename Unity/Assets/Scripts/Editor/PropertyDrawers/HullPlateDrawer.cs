using System.Collections.Generic;
using SCHIZO.HullPlates;
using UnityEditor;
using UnityEngine;

namespace PropertyDrawers
{
    [CustomPropertyDrawer(typeof(HullPlate))]
    public sealed class HullPlateDrawer : PropertyDrawer
    {
        public static List<int> NormalHullPlates = new List<int>();
        public static List<int> DeprecatedHullPlates = new List<int>();

        private static bool IsOk(SerializedProperty property)
        {
            if (!property.objectReferenceValue) return true;

            int instanceId = property.objectReferenceInstanceIDValue;
            bool isDeprecated = ((HullPlate) property.objectReferenceValue).deprecated;

            if (NormalHullPlates.Contains(instanceId) && isDeprecated) return false;
            if (DeprecatedHullPlates.Contains(instanceId) && !isDeprecated) return false;
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
