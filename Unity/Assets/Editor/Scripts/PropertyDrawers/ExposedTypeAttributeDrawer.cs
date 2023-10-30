using System;
using SCHIZO.Attributes.Typing;
using SCHIZO.Helpers;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ExposedTypeAttribute))]
    public sealed class ExposedTypeAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, DrawerUtils.ControlId(property.propertyPath + "label", position), label);

            string targetTypeName = ((ExposedTypeAttribute) attribute).typeName;
            Type actualFieldType = ReflectionCache.GetField(property.serializedObject.targetObject.GetType(), property.name).FieldType;

            if (!(ReflectionCache.GetType(targetTypeName) is Type targetType))
            {
                GUI.Label(position, "Unknown target type");
            }
            else if (!actualFieldType.IsAssignableFrom(targetType))
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
            if (typeof(Component).IsAssignableFrom(targetType))
            {
                property.objectReferenceValue = EditorGUI.ObjectField(position, GUIContent.none, property.objectReferenceValue, targetType, true);
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(targetType))
            {
                property.objectReferenceValue = EditorGUI.ObjectField(position, GUIContent.none, property.objectReferenceValue, targetType, false);
            }
        }
    }
}
