using System;
using HarmonyLib;
using SCHIZO.Packages.NaughtyAttributes;
using SCHIZO.Utilities;
using UnityEditor;
using UnityEngine;

namespace PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ValidateTypeAttribute))]
    public sealed class ValidateTypeAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, DrawerUtils.ControlId(property.propertyPath + "label", position), label);

            if (!Validate(property, attribute.))
            {
                GUI.Label(position, "Invalid type");
                return;
            }

            string targetTypeName = property.FindPropertyRelative(nameof(SubnauticaReference.typeName)).stringValue;
            if (string.IsNullOrEmpty(targetTypeName))
            {
                GUI.Label(position, "Target type is not set");
            }
            else if (!(AccessTools.TypeByName(targetTypeName) is Type targetType))
            {
                GUI.Label(position, "Unknown target type");
            }
            else
            {
                DrawCustomProperty(position, property.FindPropertyRelative(nameof(SubnauticaReference.value)), targetType);
            }

            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
        }

        private bool Validate(SerializedProperty property, ValidateTypeAttribute attribute)
        {
            if ()
        }

        private void DrawCustomProperty(Rect position, SerializedProperty property, Type targetType)
        {
            if (targetType.IsSubclassOf(typeof(Component)))
            {
                property.objectReferenceValue = EditorGUI.ObjectField(position, GUIContent.none, property.objectReferenceValue, targetType, true);
            }
            else if (targetType.IsSubclassOf(typeof(ScriptableObject)))
            {
                property.objectReferenceValue = EditorGUI.ObjectField(position, GUIContent.none, property.objectReferenceValue, targetType, false);
            }
        }
    }
}
