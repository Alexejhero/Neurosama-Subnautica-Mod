using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Nautilus.Utility;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO;

public static class AssetLoader
{
    public static readonly string AssetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Assets");

    private static readonly Dictionary<string, AssetBundle> _assetBundleCache = new();
    private static readonly Dictionary<string, Texture2D> _textureCache = new();
    private static readonly Dictionary<string, AtlasSprite> _atlasSpriteCache = new();

    public static AssetBundle GetMainAssetBundle() => GetAssetBundle("assets");

    private static AssetBundle GetAssetBundle(string name)
    {
        if (_assetBundleCache.TryGetValue(name, out AssetBundle cached)) return cached;
        return _assetBundleCache[name] = AssetBundleLoadingUtils.LoadFromAssetsFolder(Assembly.GetExecutingAssembly(), $"assetbundles/{name}")
                ?? throw new ArgumentException($"Asset bundle {name} not found", nameof(name));
    }

    public static Texture2D GetTexture(string name)
    {
        if (_textureCache.TryGetValue(name, out Texture2D cached)) return cached;
        return _textureCache[name] = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "textures", name))
                ?? throw new ArgumentException($"Texture {name} not found", nameof(name));
    }

    public static AtlasSprite GetAtlasSprite(string name)
    {
        if (_atlasSpriteCache.TryGetValue(name, out AtlasSprite cached)) return cached;
        return _atlasSpriteCache[name] = ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "textures", name))
                ?? throw new ArgumentException($"Sprite {name} not found", nameof(name));
    }

    public static Sprite GetUnitySprite(string name)
    {
        return TextureHelpers.CreateSprite(GetTexture(name));
    }
}
