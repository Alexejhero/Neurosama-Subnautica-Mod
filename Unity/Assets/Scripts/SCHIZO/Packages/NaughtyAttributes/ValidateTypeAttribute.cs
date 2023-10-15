using System;
using UnityEngine;

namespace SCHIZO.Packages.NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ValidateTypeAttribute : PropertyAttribute
    {
        public string typeName;

        public ValidateTypeAttribute(string typeName)
        {
            this.typeName = typeName;
        }
    }
}
