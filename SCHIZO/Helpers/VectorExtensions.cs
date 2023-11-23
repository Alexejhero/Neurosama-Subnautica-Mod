using UnityEngine;

namespace SCHIZO.Helpers;

public static class VectorExtensions
{
    public static Vector3 Reciprocal(this Vector3 vec3)
        => new(1f / vec3.x, 1f / vec3.y, 1f / vec3.z);
}
