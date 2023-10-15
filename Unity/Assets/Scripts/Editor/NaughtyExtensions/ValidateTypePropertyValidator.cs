using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using NaughtyAttributes.Editor;
using SCHIZO.Attributes;
using UnityEditor;
using UnityEngine;

namespace NaughtyExtensions
{
    public class ValidateTypePropertyValidator : PropertyValidatorBase
    {
        public override void ValidateProperty(SerializedProperty property)
        {
            ValidateTypeAttribute attr = PropertyUtility.GetAttribute<ValidateTypeAttribute>(property);
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                string warning = attr.GetType().Name + " works only on reference types";
                NaughtyEditorGUI.HelpBox_Layout(warning, MessageType.Warning, context: property.serializedObject.targetObject);
                return;
            }
            if (property.objectReferenceValue == null) return;

            Type expectedType = AccessTools.TypeByName(attr.typeName);

            Type actualType = property.objectReferenceValue.GetType();
            if (expectedType.IsAssignableFrom(actualType)) return;

            List<Type> sisterTypes = actualType.GetCustomAttributes<ActualTypeAttribute>().Select(a => AccessTools.TypeByName(a.typeName)).ToList();
            if (sisterTypes.Any(t => expectedType.IsAssignableFrom(t))) return;

            NaughtyEditorGUI.HelpBox_Layout($"{property.displayName} must be of type {attr.typeName}", MessageType.Error, context: property.serializedObject.targetObject);
        }
    }
}
