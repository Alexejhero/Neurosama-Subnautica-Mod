using System;

namespace NaughtyAttributes
{
    [Obsolete]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class AllowNestingAttribute : DrawerAttribute
    {
    }
}
