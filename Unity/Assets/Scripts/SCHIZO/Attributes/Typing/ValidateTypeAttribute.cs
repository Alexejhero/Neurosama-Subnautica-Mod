using System;
using System.Diagnostics;
using JetBrains.Annotations;
using NaughtyAttributes;

namespace SCHIZO.Attributes.Typing
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_STANDALONE")]
    [UsedImplicitly]
    internal sealed class ValidateTypeAttribute : ValidatorAttribute
    {
        public string typeName;

        public ValidateTypeAttribute(string typeName)
        {
            this.typeName = typeName;
        }
    }
}
