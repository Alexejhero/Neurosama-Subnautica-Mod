using System;
using System.Reflection;
using HarmonyLib;

namespace SCHIZO.Helpers
{
    public static partial class ReflectionHelpers
    {
        public static T GetMemberValue<T>(object instance, string name)
        {
            FieldInfo field = AccessTools.Field(instance.GetType(), name);
            if (field != null)
            {
                if (field.IsStatic)
                {
                    try
                    {
                        return (T) field.GetRawConstantValue();
                    }
                    catch
                    {
                        return (T) field.GetValue(null);
                    }
                }
                else
                {
                    return (T) field.GetValue(instance);
                }
            }

            PropertyInfo property = AccessTools.Property(instance.GetType(), name);
            if (property != null)
            {
                MethodInfo getMethod = property.GetGetMethod(true);
                if (getMethod.IsStatic)
                {
                    return (T) getMethod.Invoke(null, Array.Empty<object>());
                }
                else
                {
                    return (T) getMethod.Invoke(instance, Array.Empty<object>());
                }
            }

            MethodInfo method = AccessTools.Method(instance.GetType(), name);
            if (method != null)
            {
                if (method.IsStatic)
                {
                    return (T) method.Invoke(null, Array.Empty<object>());
                }
                else
                {
                    return (T) method.Invoke(instance, Array.Empty<object>());
                }
            }

            throw new Exception($"Failed to retrieve member value with name {name}");
        }
    }
}
