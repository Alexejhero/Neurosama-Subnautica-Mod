using System;
using NaughtyAttributes;

namespace SCHIZO.Attributes.Typing
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ValidateTypeAttribute : ValidatorAttribute
    {
        public string typeName;

        public ValidateTypeAttribute(string typeName)
        {
            this.typeName = typeName;
        }
    }
}
