using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SCHIZO.Extensions;
using UnityEngine;

namespace SCHIZO.Resources;

public static class ResourceManager
{
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
    private static readonly Dictionary<string, object> _cache = new();

    public static T LoadAsset<T>(string name) where T : UnityEngine.Object
    {
        return GetAssetBundle("assets").LoadAsset<T>(name) ?? throw new ArgumentException($"Asset {name} not found in asset bundle", nameof(name));
    }

    private static AssetBundle GetAssetBundle(string name)
    {
        string targetedName = "AssetBundles." + name;

        if (IsCached(targetedName, out AssetBundle cached)) return cached;

        byte[] buffer = GetEmbeddedBytes(targetedName);
        AssetBundle assetBundle = AssetBundle.LoadFromMemory(buffer);
        return Cache(targetedName, assetBundle);
    }

    private static byte[] GetEmbeddedBytes(string name)
    {
        string path = _assembly.GetManifestResourceNames().Single(n => n.Contains(name));

        Stream manifestResourceStream = _assembly.GetManifestResourceStream(path)!;
        return manifestResourceStream.ReadFully();
    }

    private static bool IsCached<T>(string name, out T value) where T : class
    {
        if (!_cache.TryGetValue(name, out object cachedObject))
        {
            value = null;
            return false;
        }

        if (cachedObject is not T correctType) throw new InvalidCastException($"Cached object is not of type {typeof(T)}");

        value = correctType;
        return true;
    }

    private static T Cache<T>(string name, T obj)
    {
        if (_cache.ContainsKey(name)) throw new InvalidOperationException($"Cache already contains an object with name {name}");

        _cache.Add(name, obj);

        return obj;
    }
}
