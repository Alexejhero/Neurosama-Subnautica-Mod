using System;
using System.Collections.Generic;
using System.Reflection;
using Editor.Scripts.NaughtyExtensions.Validators;
using HarmonyLib;
using JetBrains.Annotations;
using NaughtyAttributes;
using NaughtyAttributes.Editor;
using SCHIZO.Attributes.Typing;
using SCHIZO.Attributes.Validation;

namespace Editor.Scripts.Patches
{
    [HarmonyPatch, UsedImplicitly]
    public static class CustomValidatorAttributes
    {
        private static readonly FieldInfo _validatorsByAttributeType_field = AccessTools.Field(typeof(ValidatorAttributeExtensions), "_validatorsByAttributeType");
        private static Dictionary<Type, PropertyValidatorBase> _originalPropertyValidators => (Dictionary<Type, PropertyValidatorBase>) _validatorsByAttributeType_field.GetValue(null);

        private static readonly Dictionary<Type, PropertyValidatorBase> _customPropertyValidators = new Dictionary<Type, PropertyValidatorBase>()
        {
            {typeof(ValidateTypeAttribute), new ValidateTypePropertyValidator()},
            {typeof(FlexibleValidateInputAttribute), new FlexibleValidateInputPropertyValidator()},
            {typeof(object), null},
        };

        [HarmonyPatch(typeof(ValidatorAttributeExtensions), nameof(ValidatorAttributeExtensions.GetValidator)), UsedImplicitly]
        [HarmonyPrefix]
        public static bool GetCustomValidator(out PropertyValidatorBase __result, ValidatorAttribute attr)
        {
            __result = GetValidator(attr.GetType());
            return __result == null;
        }

        private static PropertyValidatorBase GetValidator(Type type)
        {
            if (_customPropertyValidators.TryGetValue(type, out PropertyValidatorBase customValidator)) return customValidator;
            if (_originalPropertyValidators.TryGetValue(type, out PropertyValidatorBase originalValidator)) return originalValidator;
            return GetValidator(type.BaseType);
        }
    }
}
