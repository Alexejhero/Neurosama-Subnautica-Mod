using System.Reflection;

namespace Immersion.Resources;

public static class ResourceManager
{
    public static void InjectAssemblies()
    {
        AppDomain.CurrentDomain.AssemblyResolve += (_, args) => GetAssembly(new AssemblyName(args.Name).Name);
    }

    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
    private static readonly Dictionary<string, object> _cache = new();

    public static AssetBundle GetAssetBundle(string name)
    {
        string targetedName = "AssetBundles." + name;

        if (TryGetCached(targetedName, out AssetBundle cached)) return cached;

        byte[] buffer = GetEmbeddedBytes(targetedName, true);
        AssetBundle assetBundle = AssetBundle.LoadFromMemory(buffer);
        return Cache(targetedName, assetBundle);
    }

    public static Assembly GetAssembly(string name)
    {
        string targetedName = name + ".dll";

        if (TryGetCached(targetedName, out Assembly cached)) return cached;

        byte[] buffer = GetEmbeddedBytes(targetedName, false);
        if (buffer == null) return null;

        Assembly assembly = Assembly.Load(buffer);
        return Cache(targetedName, assembly);
    }

    internal static byte[] GetEmbeddedBytes(string name, bool throwIfNotFound)
    {
        string path = _assembly.GetManifestResourceNames().FirstOrDefault(n => n.Contains(name));
        if (path == null) return !throwIfNotFound ? null : throw new FileNotFoundException($"Embedded resource {name} not found");

        Stream manifestResourceStream = _assembly.GetManifestResourceStream(path)!;
        return manifestResourceStream.ReadFully();
    }

    private static bool TryGetCached<T>(string name, out T value) where T : class
    {
        if (!_cache.TryGetValue(name, out object cachedObject))
        {
            value = null;
            return false;
        }

        value = cachedObject as T ?? throw new InvalidCastException($"Cached object is not of type {typeof(T)}");
        return true;
    }

    private static T Cache<T>(string name, T obj)
    {
        _cache.Add(name, obj);

        return obj;
    }

    private static byte[] ReadFully(this Stream stream)
    {
        using MemoryStream ms = new();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}
