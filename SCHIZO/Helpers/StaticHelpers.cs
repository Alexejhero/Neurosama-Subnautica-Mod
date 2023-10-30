using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using NaughtyAttributes;

namespace SCHIZO.Helpers;

partial class StaticHelpers
{
    partial class CacheAttribute
    {
        public static void CacheAll()
        {
            PLUGIN_ASSEMBLY.GetTypes().SelectMany(t => t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                .Where(f => f.GetCustomAttribute<CacheAttribute>() != null)
                .Select(f => (IDropdownList) f.GetValue(null))
                .Select(d => d.Select(kvp => (string) kvp.Value))
                .ForEach(CacheValues);
        }
    }

    private static readonly Dictionary<string, object> _cache = new();

    public static T GetValue<T>(string fieldOrPropertyColonName)
    {
        if (TryGetCached(fieldOrPropertyColonName, out T result)) return result;
        throw new Exception($"Failed to retrieve static value with name {fieldOrPropertyColonName}. Did you cache it first?");
    }

    private static void CacheValues(IEnumerable<string> colonNames)
    {
        foreach (string colonName in colonNames)
        {
            try
            {
                LOGGER.LogDebug("Caching " + colonName);
                Cache(colonName);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to cache static value with name {colonName}", e);
            }
        }
    }

    private static bool TryGetCached<T>(string name, out T value)
    {
        if (!_cache.TryGetValue(name, out object cachedObject))
        {
            value = default;
            return false;
        }

        value = (T) cachedObject;
        return true;
    }

    private static void Cache(string fieldOrPropertyColonName)
    {
        string[] splits = fieldOrPropertyColonName.Split(':');
        Type type = AccessTools.TypeByName(splits[0]);

        FieldInfo f = type.GetField(splits[1], BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        if (f != null)
        {
            if (f.Attributes.HasFlag(FieldAttributes.Literal | FieldAttributes.HasDefault))
            {
                _cache.Add(fieldOrPropertyColonName, f.GetRawConstantValue());
            }
            else
            {
                _cache.Add(fieldOrPropertyColonName, f.GetValue(null));
            }

            return;
        }

        MethodInfo p = type.GetProperty(splits[1], BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!.GetGetMethod(true);

        _cache.Add(fieldOrPropertyColonName, p.Invoke(null, Array.Empty<object>()));
    }
}
