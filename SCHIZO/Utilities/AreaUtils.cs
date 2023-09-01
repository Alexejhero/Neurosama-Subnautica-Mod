using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SCHIZO.Utilities;

public static class AreaUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<GameObject> ObjectsInRange(GameObject center, float radius)
        => ObjectsInRange(center.transform.position, radius);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<GameObject> ObjectsInRange(Transform center, float radius)
        => ObjectsInRange(center.position, radius);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<GameObject> ObjectsInRange(Vector3 center, float radius)
    {
        return Physics.OverlapSphere(center, radius)
            .Select(collider => collider.gameObject);
    }
}
