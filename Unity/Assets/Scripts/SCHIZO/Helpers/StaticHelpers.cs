using System;

namespace SCHIZO.Helpers
{
    public static partial class StaticHelpers
    {
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
        public sealed partial class CacheAttribute : Attribute
        {
        }
    }
}
