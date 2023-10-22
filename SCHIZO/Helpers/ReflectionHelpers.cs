using System;
using System.Reflection;
using HarmonyLib;

namespace SCHIZO.Helpers;

partial class ReflectionHelpers
{
    public static T GetStaticValue<T>(string fieldOrPropertyColonName)
    {
        try
        {
            return GetStaticValueUnsafe<T>(fieldOrPropertyColonName);
        }
        catch (Exception)
        {
            LOGGER.LogFatal("Failed to retrieve static value with name " + fieldOrPropertyColonName);
            throw;
        }
    }

    private static T GetStaticValueUnsafe<T>(string fieldOrPropertyColonName)
    {
        string[] splits = fieldOrPropertyColonName.Split(':');
        Type type = AccessTools.TypeByName(splits[0]);

        FieldInfo f = type.GetField(splits[1], BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        if (f != null)
        {
            try
            {
                return (T) f.GetRawConstantValue();
            }
            catch
            {
                return (T) f.GetValue(null);
            }
        }

        MethodInfo p = type.GetProperty(splits[1], BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!.GetGetMethod(true);
        return (T) p.Invoke(null, Array.Empty<object>());
    }
}
