using System;
using UnityEngine;

namespace SCHIZO.Utilities;

public static class Extensions
{
    public static T LoadAssetSafe<T>(this AssetBundle bundle, string name) where T : UnityEngine.Object
    {
        return bundle.LoadAsset<T>(name) ?? throw new ArgumentException($"Asset {name} not found in bundle {bundle.name}", nameof(name));
    }
}
