using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace SCHIZO.Helpers
{
    public static class ReflectionCache
    {
        private const BindingFlags ALL = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private class Cache<TKey, TValue>(Func<TKey, TValue> getValueFunc)
        {
            private readonly Dictionary<TKey, TValue> _cache = [];

            public TValue GetCached(TKey key)
            {
                if (!_cache.TryGetValue(key, out TValue value))
                    value = _cache[key] = getValueFunc(key);
                return value;
            }
        }
        private static readonly Cache<string, Type> _types = new(AccessTools.TypeByName);
        private static readonly Cache<Type, List<FieldInfo>> _allFields = new(
            type => ReflectionHelpers.WalkTypeHierarchy(type).SelectMany(t => t.GetFields(ALL)).ToList()
        );
        private static readonly Cache<(Type type, string name), FieldInfo> _fieldByName = new(
            pair => AccessTools.Field(pair.type, pair.name)
        );
        private static readonly Cache<Type, List<MethodInfo>> _allMethods = new(
            type => ReflectionHelpers.WalkTypeHierarchy(type).SelectMany(t => t.GetMethods(ALL)).ToList()
        );
        private static readonly Cache<(Type type, string name), MethodInfo> _methodByName = new(
            pair => AccessTools.Method(pair.type, pair.name)
        );
        private static readonly Cache<Type, List<PropertyInfo>> _allProperties = new(
            type => ReflectionHelpers.WalkTypeHierarchy(type).SelectMany(t => t.GetProperties(ALL)).ToList()
        );
        private static readonly Cache<(Type type, string name), PropertyInfo> _propertyByName = new(
            pair => AccessTools.Property(pair.type, pair.name)
        );
        private static readonly Cache<MemberInfo, List<Attribute>> _memberAttrs = new(member => member.GetCustomAttributes().ToList());

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
