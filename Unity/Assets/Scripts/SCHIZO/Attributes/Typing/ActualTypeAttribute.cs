using System;

namespace SCHIZO.Attributes.Typing
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
