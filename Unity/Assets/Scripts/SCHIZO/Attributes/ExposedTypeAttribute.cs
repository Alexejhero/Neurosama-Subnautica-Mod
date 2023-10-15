using System;
using UnityEngine;

namespace SCHIZO.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ExposedTypeAttribute : PropertyAttribute
    {
        public string typeName;

        public ExposedTypeAttribute(string typeName)
        {
            this.typeName = typeName;
        }
    }
}
