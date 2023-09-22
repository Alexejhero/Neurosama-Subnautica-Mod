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

    private static readonly Dictionary<string, Texture2D> _textureCache = new();

    [Obsolete]
    public static Texture2D GetTexture(string name)
    {
        if (_textureCache.TryGetValue(name, out Texture2D cached)) return cached;
        return _textureCache[name] = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "textures", name))
                ?? throw new ArgumentException($"Texture {name} not found", nameof(name));
    }

    [Obsolete]
    public static Sprite GetUnitySprite(string name)
    {
        return TextureHelpers.CreateSprite(GetTexture(name));
    }
}
