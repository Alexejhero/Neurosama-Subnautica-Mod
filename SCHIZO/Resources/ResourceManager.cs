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

    public static AssetBundle GetAssetBundle(string name)
    {
        if (IsCached(name, out AssetBundle cached)) return cached;

        byte[] buffer = GetEmbeddedBytes(name);
        AssetBundle assetBundle = AssetBundle.LoadFromMemory(buffer);
        return Cache(name, assetBundle);
    }

    public static Texture2D GetTexture(string name)
    {
        if (IsCached(name, out Texture2D cached)) return cached;

        byte[] buffer = GetEmbeddedBytes(name);
        Texture2D tex = new(2, 2, TextureFormat.ARGB32, false);
        tex.LoadImage(buffer, false);
        return Cache(name, tex);
    }

    public static Sprite GetUnitySprite(string name, float ppu = 100)
    {
        Texture2D tex = GetTexture(name);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), ppu);
    }

    public static AtlasSprite GetAtlasSprite(string name)
    {
#if SUBNAUTICA
        Texture2D tex = GetTexture(name);
        return new AtlasSprite(tex);
#else
        return GetUnitySprite(name);
#endif
    }

    /*public static AudioClip GetSound(string name, Bus bus, MODE mode = MODE.DEFAULT)
    {
        byte[] buffer = GetEmbeddedBytes(name);

        if (bus.getChannelGroup(out ChannelGroup _) != RESULT.OK)
            bus.lockChannelGroup().CheckResult();

        CREATESOUNDEXINFO info = new();
        AudioUtils.FMOD_System.createSound(buffer, mode, ref info, out Sound sound);

        CustomSoundPatcher.CustomSounds[id] = sound;
        CustomSoundPatcher.CustomSoundBuses[id] = bus;

        return sound;
    }*/

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
