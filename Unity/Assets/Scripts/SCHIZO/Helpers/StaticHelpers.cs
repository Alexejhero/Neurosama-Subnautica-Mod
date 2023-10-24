using System;

namespace SCHIZO.Helpers
{
    public static partial class StaticHelpers
    {
        [AttributeUsage(AttributeTargets.Field)]
        public sealed partial class CacheAttribute : Attribute
        {
        }
    }
}
