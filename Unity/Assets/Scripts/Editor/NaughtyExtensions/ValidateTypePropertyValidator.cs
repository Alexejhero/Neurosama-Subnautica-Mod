using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using NaughtyAttributes.Editor;
using SCHIZO.Attributes.Typing;
using UnityEditor;
using UnityEngine.Profiling;

namespace NaughtyExtensions
{
    public class ValidateTypePropertyValidator : PropertyValidatorBase
    {
        private static readonly Dictionary<SerializedProperty, object> _changedCache = new Dictionary<SerializedProperty, object>();

        public override void ValidateProperty(SerializedProperty property)
        {
            Profiler.BeginSample($"{nameof(ValidateTypePropertyValidator)}.{nameof(ValidateProperty)}");
            Inner();
            Profiler.EndSample();

            // no, this does not allocate
            void Inner()
            {
                ValidateTypeAttribute attr = PropertyUtility.GetAttribute<ValidateTypeAttribute>(property);
                if (property.propertyType != SerializedPropertyType.ObjectReference)
                {
                    string warning = attr.GetType().Name + " works only on reference types";
                    NaughtyEditorGUI.HelpBox_Layout(warning, MessageType.Warning, context: property.serializedObject.targetObject);
                    return;
                }

                if (_changedCache.TryGetValue(property, out object value)
                    && ReferenceEquals(property.objectReferenceValue, value)) return;
                _changedCache[property] = property.objectReferenceValue;

                if (property.objectReferenceValue == null) return;

                Type expectedType = ReflectionCache.GetType(attr.typeName);

                Type actualType = property.objectReferenceValue.GetType();
                if (expectedType.IsAssignableFrom(actualType)) return;

                List<Type> sisterTypes = actualType.GetCustomAttributes<ActualTypeAttribute>().Select(a => ReflectionCache.GetType(a.typeName)).ToList();
                if (sisterTypes.Any(t => expectedType.IsAssignableFrom(t))) return;

                NaughtyEditorGUI.HelpBox_Layout($"{property.displayName} must be of type {attr.typeName}", MessageType.Error, context: property.serializedObject.targetObject);
            }
        }
    }
}
