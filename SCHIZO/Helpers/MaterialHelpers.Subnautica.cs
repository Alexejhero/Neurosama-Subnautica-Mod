using System.Collections;
using Nautilus.Utility;
using UnityEngine;

namespace SCHIZO.Helpers;

public static partial class MaterialHelpers
{
    private static bool _isReady => MaterialUtils.IsReady;

    public static Material GhostMaterial => MaterialUtils.GhostMaterial;

    public static IEnumerator LoadMaterials()
    {
        while (!_isReady) yield return null;
    }
}
