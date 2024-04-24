namespace SCHIZO.Helpers;

internal static class ObjectHelpers
{
    public static bool Exists(this object obj)
        => obj is { } && !(obj is UnityEngine.Object unityObj && !unityObj);
}
