using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TriInspector;

namespace SCHIZO.Helpers;

partial class StaticHelpers
{
    partial class CacheAttribute
    {
        public static void CacheAll()
        {
            IEnumerable<string> names = PLUGIN_ASSEMBLY.GetTypes()
                .SelectMany(t => t.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                .Where(m => m.GetCustomAttribute<CacheAttribute>() != null)
                .Select(ReflectionHelpers.GetStaticMemberValue<IEnumerable>)
                .SelectMany(i => i.Cast<ITriDropdownItem>())
                .Select(i => i.Value.ToString());

            CacheValues(names);
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
        Type type = ReflectionCache.GetType(splits[0]);

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
