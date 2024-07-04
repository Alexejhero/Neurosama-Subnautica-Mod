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
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            return GetMemberValue<T>(instance.GetType(), instance, name);
        }

        public static T GetMemberValue<T>(Type type, object instance, string name)
        {
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
                return (T) method.Invoke(thisArg, null);
            }

            throw new Exception($"Failed to retrieve member value with name {name}");
        }

        public static T GetStaticMemberValue<T>(MemberInfo member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (member is FieldInfo field)
            {
                if (!typeof(T).IsAssignableFrom(field.FieldType))
                    throw new ArgumentException($"Member (field) type {field.FieldType} cannot be assigned to {typeof(T)}");

                return (T) field.GetValue(null);
            }
            if (member is PropertyInfo property)
            {
                if (!typeof(T).IsAssignableFrom(property.PropertyType))
                    throw new ArgumentException($"Member (property) type {property.PropertyType} cannot be assigned to {typeof(T)}");
                if (!property.CanRead)
                    throw new ArgumentException("Member (property) cannot be read");

                return (T) property.GetValue(null);
            }
            if (member is MethodInfo method)
            {
                if (!typeof(T).IsAssignableFrom(method.ReturnType))
                    throw new ArgumentException($"Member (method) return type {method.ReturnType} cannot be assigned to {typeof(T)}");
                if (method.GetParameters().Length != 0)
                    throw new ArgumentException("Member (method) must have no parameters");
                return (T) method.Invoke(null, null);
            }
            throw new NotSupportedException($"Unsupported member type {member.GetType().Name}");
        }
    }
}
