﻿using System;
using HarmonyLib;
using SCHIZO.Packages.NaughtyAttributes;
using SCHIZO.Utilities;
using UnityEditor;
using UnityEngine;

namespace PropertyDrawers
{
    // [CustomPropertyDrawer(typeof(SubnauticaSerializableClass))]
    public sealed class SubnauticaSerializableClassDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, DrawerUtils.ControlId(property.propertyPath + "label", position), label);

            string targetTypeName = ((ValidateTypeAttribute) attribute).typeName;
            Type actualFieldType = AccessTools.Field(property.serializedObject.targetObject.GetType(), property.name).FieldType;

            if (!(AccessTools.TypeByName(targetTypeName) is Type targetType))
            {
                GUI.Label(position, "Unknown target type");
            }
            else if (!targetType.IsSubclassOf(actualFieldType))
            {
                GUI.Label(position, "Field has incompatible type");
            }
            else
            {
                DrawCustomProperty(position, property, targetType);
            }

            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
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
