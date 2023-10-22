using System;
using System.Collections.Generic;
using System.Reflection;

namespace SCHIZO.Helpers
{
    public static partial class ReflectionHelpers
    {
        public static IEnumerable<Type> WalkTypeHierarchy(Type leaf)
        {
            for (Type curr = leaf; curr != null; curr = curr.BaseType)
                yield return curr;
        }
        public static T GetMemberValue<T>(object instance, string name)
        {
            Type type = instance.GetType();

            FieldInfo field = ReflectionCache.GetField(type, name);
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

            PropertyInfo property = ReflectionCache.GetProperty(type, name);
            MethodInfo method = property?.GetGetMethod(true) ?? ReflectionCache.GetMethod(type, name);

            if (method != null) 
            {
                object thisArg = method.IsStatic ? null : instance;
                return (T) method.Invoke(thisArg, Array.Empty<object>());
            }

            throw new Exception($"Failed to retrieve member value with name {name}");
        }
    }
}
