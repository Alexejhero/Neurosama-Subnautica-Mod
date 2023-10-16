using System;
using System.Diagnostics;

namespace SCHIZO.Attributes.Typing
{
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("UNITY_STANDALONE")]
    internal sealed class ActualTypeAttribute : Attribute
    {
        public string typeName;

        public ActualTypeAttribute(string typeName)
        {
            this.typeName = typeName;
        }
    }
}
