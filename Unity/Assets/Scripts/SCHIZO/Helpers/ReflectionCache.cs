using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SCHIZO.Helpers
{
    public static class ReflectionCache
    {
        private const BindingFlags ALL = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private class Cache<TKey, TValue>
        {
            private readonly Func<TKey, TValue> getValueFunc;
            private readonly Dictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();
            public Cache(Func<TKey, TValue> getValueFunc)
            {
                this.getValueFunc = getValueFunc;
            }
            public TValue GetCached(TKey key)
            {
                if (!_cache.TryGetValue(key, out TValue value))
                    value = _cache[key] = getValueFunc(key);
                return value;
            }
        }
        private static readonly Cache<string, Type> _types = new Cache<string, Type>(
            // condensed version of AccessTools.TypeByName (we can't use HarmonyLib here)
            typeName => Type.GetType(typeName, false)
                ?? AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .FirstOrDefault(t => t.Name == typeName || t.FullName == typeName)
        );
        private static readonly Cache<Type, List<FieldInfo>> _allFields = new Cache<Type, List<FieldInfo>>(
            type => ReflectionHelpers.WalkTypeHierarchy(type)
                .SelectMany(t => t.GetFields(ALL))
                .ToList()
        );
        private static readonly Cache<(Type, string), FieldInfo> _fieldByName = new Cache<(Type, string), FieldInfo>(
            pair => ReflectionHelpers.WalkTypeHierarchy(pair.Item1)
                .Select(t => t.GetField(pair.Item2, ALL))
                .FirstOrDefault(f => f != null)
        );
        private static readonly Cache<Type, List<MethodInfo>> _allMethods = new Cache<Type, List<MethodInfo>>(
            type => ReflectionHelpers.WalkTypeHierarchy(type)
                .SelectMany(t => t.GetMethods(ALL))
                .ToList()
        );
        private static readonly Cache<(Type, string), MethodInfo> _methodByName = new Cache<(Type, string), MethodInfo>(
            pair => ReflectionHelpers.WalkTypeHierarchy(pair.Item1)
                .Select(t => t.GetMethod(pair.Item2, ALL))
                .FirstOrDefault(f => f != null)
        );
        private static readonly Cache<Type, List<PropertyInfo>> _allProperties = new Cache<Type, List<PropertyInfo>>(
            type => ReflectionHelpers.WalkTypeHierarchy(type)
                .SelectMany(t => t.GetProperties(ALL))
                .ToList()
        );
        private static readonly Cache<(Type, string), PropertyInfo> _propertyByName = new Cache<(Type, string), PropertyInfo>(
            pair => ReflectionHelpers.WalkTypeHierarchy(pair.Item1)
                .Select(t => t.GetProperty(pair.Item2, ALL))
                .FirstOrDefault(f => f != null)
        );
        private static readonly Cache<MemberInfo, List<Attribute>> _memberAttrs = new Cache<MemberInfo, List<Attribute>>(member => member.GetCustomAttributes().ToList());

        public static Type GetType(string typeName) => _types.GetCached(typeName);
        public static List<FieldInfo> GetAllFields(Type type) => _allFields.GetCached(type);
        public static FieldInfo GetField(Type type, string name) => _fieldByName.GetCached((type, name));
        public static List<MethodInfo> GetAllMethods(Type type) => _allMethods.GetCached(type);
        public static MethodInfo GetMethod(Type type, string name) => _methodByName.GetCached((type, name));
        public static List<PropertyInfo> GetAllProperties(Type type) => _allProperties.GetCached(type);
        public static PropertyInfo GetProperty(Type type, string name) => _propertyByName.GetCached((type, name));

        public static List<Attribute> GetCustomAttributes(MemberInfo member) => _memberAttrs.GetCached(member);
    }
}
