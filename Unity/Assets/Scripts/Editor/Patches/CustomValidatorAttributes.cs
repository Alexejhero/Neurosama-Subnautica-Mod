using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using NaughtyAttributes;
using NaughtyAttributes.Editor;
using NaughtyExtensions;
using SCHIZO.Attributes;

namespace Patches
{
    [HarmonyPatch, UsedImplicitly]
    public static class CustomValidatorAttributes
    {
        private static readonly Dictionary<Type, PropertyValidatorBase> _customPropertyValidators = new Dictionary<Type, PropertyValidatorBase>()
        {
            {typeof(ValidateTypeAttribute), new ValidateTypePropertyValidator()}
        };

        [HarmonyPatch(typeof(ValidatorAttributeExtensions), nameof(ValidatorAttributeExtensions.GetValidator)), UsedImplicitly]
        [HarmonyPrefix]
        public static bool GetCustomValidator(out PropertyValidatorBase __result, ValidatorAttribute attr)
        {
            return !_customPropertyValidators.TryGetValue(attr.GetType(), out __result);
        }
    }
}
