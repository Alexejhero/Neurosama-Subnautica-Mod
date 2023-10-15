using System;

namespace SCHIZO.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ActualTypeAttribute : Attribute
    {
        public string typeName;

        public ActualTypeAttribute(string typeName)
        {
            this.typeName = typeName;
        }
    }
}
