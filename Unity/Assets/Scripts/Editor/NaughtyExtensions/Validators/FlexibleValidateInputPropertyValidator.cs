using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using NaughtyAttributes.Editor;
using SCHIZO.Attributes.Validation;
using UnityEditor;

namespace NaughtyExtensions.Validators
{
    public sealed class FlexibleValidateInputPropertyValidator : PropertyValidatorBase
    {
        private static readonly Dictionary<SerializedProperty, object> _changedCache = new Dictionary<SerializedProperty, object>();

        public override void ValidateProperty(SerializedProperty property)
        {
            FlexibleValidateInputAttribute attr = PropertyUtility.GetAttribute<FlexibleValidateInputAttribute>(property);

            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                string warning = attr.GetType().Name + " works only on reference types";
                NaughtyEditorGUI.HelpBox_Layout(warning, MessageType.Warning, context: property.serializedObject.targetObject);
                return;
            }

            object propertyCurrentValue = GetAnyValue(property);

            if (_changedCache.TryGetValue(property, out object value))
            {
                if (ReferenceEquals(propertyCurrentValue, value)) return;
                _changedCache[property] = propertyCurrentValue;
            }

            string err = "";
            if (attr.ValidateInput(propertyCurrentValue, property, ref err)) return;

            NaughtyEditorGUI.HelpBox_Layout(err, MessageType.Error, context: property.serializedObject.targetObject);
        }

        private static readonly MethodInfo get_gradientValue = AccessTools.PropertyGetter(typeof(SerializedProperty), "gradientValue");

        private static object GetAnyValue(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                // case SerializedPropertyType.Generic:

                case SerializedPropertyType.Integer:
                    return property.intValue;

                case SerializedPropertyType.Boolean:
                    return property.boolValue;

                case SerializedPropertyType.Float:
                    return property.floatValue;

                case SerializedPropertyType.String:
                    return property.stringValue;

                case SerializedPropertyType.Color:
                    return property.colorValue;

                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue;

                case SerializedPropertyType.LayerMask:
                    return property.intValue;

                case SerializedPropertyType.Enum:
                    return property.enumValueIndex;

                case SerializedPropertyType.Vector2:
                    return property.vector2Value;

                case SerializedPropertyType.Vector3:
                    return property.vector3Value;

                case SerializedPropertyType.Vector4:
                    return property.vector4Value;

                case SerializedPropertyType.Rect:
                    return property.rectValue;

                case SerializedPropertyType.ArraySize:
                    return property.arraySize;

                case SerializedPropertyType.Character:
                    return (char)property.intValue;

                case SerializedPropertyType.AnimationCurve:
                    return property.animationCurveValue;

                case SerializedPropertyType.Bounds:
                    return property.boundsValue;

                case SerializedPropertyType.Gradient:
                    return get_gradientValue.Invoke(property, Array.Empty<object>());

                case SerializedPropertyType.Quaternion:
                    return property.quaternionValue;

                case SerializedPropertyType.ExposedReference:
                    return property.exposedReferenceValue;

                case SerializedPropertyType.FixedBufferSize:
                    return property.fixedBufferSize;

                case SerializedPropertyType.Vector2Int:
                    return property.vector2IntValue;

                case SerializedPropertyType.Vector3Int:
                    return property.vector3IntValue;

                case SerializedPropertyType.RectInt:
                    return property.rectIntValue;

                case SerializedPropertyType.BoundsInt:
                    return property.boundsIntValue;

                // case SerializedPropertyType.ManagedReference:
                    // return property.managedReferenceValue;

                default:
                    return property; // caching not supported
            }
        }
    }
}
