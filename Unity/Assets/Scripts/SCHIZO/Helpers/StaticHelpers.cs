using System;

namespace SCHIZO.Helpers
{
    public static partial class StaticHelpers
    {
        [AttributeUsage(AttributeTargets.Method)]
        public sealed partial class CacheAttribute : Attribute
        {
        }
    }
}
