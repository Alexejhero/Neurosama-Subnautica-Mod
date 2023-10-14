using System;
using NaughtyAttributes;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.NaughtyExtensions
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
