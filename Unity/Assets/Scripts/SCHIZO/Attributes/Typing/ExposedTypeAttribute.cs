using System;
using System.Diagnostics;
using UnityEngine;

namespace SCHIZO.Attributes.Typing
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_STANDALONE")]
    internal sealed class ExposedTypeAttribute : PropertyAttribute
    {
        public string typeName;

        public ExposedTypeAttribute(string typeName)
        {
            this.typeName = typeName;
        }
    }
}
