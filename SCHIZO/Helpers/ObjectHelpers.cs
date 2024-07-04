using System;

namespace SCHIZO.Helpers;

internal static class ObjectHelpers
{
    public static bool Exists(this object obj)
        => obj is { } && !(obj is UnityEngine.Object unityObj && !unityObj);

    /// <summary>
    /// Replacement for null-coalescing for Unity.<br/>
    /// <b>Does not short-circuit.</b> For short-circuiting behaviour, use <see cref="Or{T}(T, Func{T})"/> instead.
    /// </summary>
    public static T Or<T>(this T obj, T other)
        where T : UnityEngine.Object
    {
        if (obj) return obj;
        return other;
    }

    /// <summary>
    /// Replacement for null-coalescing for Unity.<br/>
    /// Use to emulate short-circuiting behaviour.
    /// </summary>
    public static T Or<T>(this T obj, Func<T> other)
        where T : UnityEngine.Object
    {
        if (obj) return obj;
        return other();
    }
}
