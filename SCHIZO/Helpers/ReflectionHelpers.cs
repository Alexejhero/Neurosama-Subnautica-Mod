using System;

namespace SCHIZO.Helpers;

public static partial class ReflectionHelpers
{
    public static bool IsStatic(this Type type)
        => type.IsAbstract && type.IsSealed;
}
