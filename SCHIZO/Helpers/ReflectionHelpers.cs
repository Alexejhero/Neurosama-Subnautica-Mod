using System.Reflection;
using HarmonyLib;

namespace SCHIZO.Helpers;

public static class ReflectionHelpers
{
    public static T GetFieldValue<T>(string fieldColonName)
    {
        string[] splits = fieldColonName.Split(':');
        FieldInfo field = AccessTools.Field(AccessTools.TypeByName(splits[0]), splits[1]);

        try
        {
            return (T) field.GetRawConstantValue();
        }
        catch
        {
            return (T) field.GetValue(null);
        }
    }
}
