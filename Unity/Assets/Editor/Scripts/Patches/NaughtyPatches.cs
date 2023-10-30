using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using NaughtyAttributes.Editor;
using SCHIZO.Helpers;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.Patches
{
    [HarmonyPatch]
    public static class NaughtyPatches
    {
        [HarmonyPatch(typeof(ReflectionUtility))]
        public static class ReflectionPerfDetours
        {
            [HarmonyPatch(nameof(ReflectionUtility.GetField))]
            [HarmonyPrefix]
            public static bool FieldLookupPerfDetour(out FieldInfo __result, object target, string fieldName)
            {
                __result = null;
                if (target != null) __result = ReflectionCache.GetField(target.GetType(), fieldName);

                return false;
            }

            [HarmonyPatch(nameof(ReflectionUtility.GetMethod))]
            [HarmonyPrefix]
            public static bool MethodLookupPerfDetour(out MethodInfo __result, object target, string methodName)
            {
                __result = null;
                if (target != null) __result = ReflectionCache.GetMethod(target.GetType(), methodName);

                return false;
            }

            [HarmonyPatch(nameof(ReflectionUtility.GetProperty))]
            [HarmonyPrefix]
            public static bool PropertyLookupPerfDetour(out PropertyInfo __result, object target, string propertyName)
            {
                __result = null;
                if (target != null) __result = ReflectionCache.GetProperty(target.GetType(), propertyName);

                return false;
            }

            [HarmonyPatch("GetSelfAndBaseTypes")]
            [HarmonyPrefix]
            public static bool TypeHierarchyPerfDetour(out List<Type> __result, object target)
            {
                __result = ReflectionHelpers.WalkTypeHierarchy(target?.GetType()).ToList();
                return false;
            }
        }

        [HarmonyPatch(typeof(NaughtyInspector), "OnEnable")]
        [HarmonyPostfix]
        public static void EnumerablePerfDetour(NaughtyInspector __instance,
            ref IEnumerable<FieldInfo> ____nonSerializedFields,
            ref IEnumerable<PropertyInfo> ____nativeProperties,
            ref IEnumerable<MethodInfo> ____methods)
        {
            if (!__instance.target) return;

            ____nonSerializedFields = ____nonSerializedFields.ToList();
            ____nativeProperties = ____nativeProperties.ToList();
            ____methods = ____methods.ToList();
        }

        [HarmonyPatch(typeof(PropertyUtility), nameof(PropertyUtility.GetLabel))]
        [HarmonyPostfix]
        public static void AddTooltip(SerializedProperty property, ref GUIContent __result)
        {
            TooltipAttribute tooltip = PropertyUtility.GetAttribute<TooltipAttribute>(property);
            if (tooltip != null) __result.tooltip = tooltip.tooltip;
        }
    }
}
