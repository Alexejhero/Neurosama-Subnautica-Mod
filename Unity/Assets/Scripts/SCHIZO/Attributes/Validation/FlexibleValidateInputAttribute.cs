using System;
using NaughtyAttributes;
using SCHIZO.Interop.NaughtyAttributes;

namespace SCHIZO.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Field)]
    internal abstract class FlexibleValidateInputAttribute : ValidatorAttribute
    {
        public abstract bool ValidateInput(object val, SerializedPropertyHolder propertyHolder, ref string err);
    }
}
