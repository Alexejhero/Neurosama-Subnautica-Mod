using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SCHIZO.Extensions;

public static class ResourceExtensions
{
    public static T LoadAssetSafe<T>(this AssetBundle bundle, string name) where T : Object
    {
        return bundle.LoadAsset<T>(name) ?? throw new ArgumentException($"Asset {name} not found in bundle {bundle.name}", nameof(name));
    }
}
