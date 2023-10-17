using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using NaughtyAttributes.Editor;

public static class ReflectionCache
{
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
    private static readonly Cache<string, Type> _types = new Cache<string, Type>(typeName => AccessTools.TypeByName(typeName));
    private static readonly Cache<Type, List<FieldInfo>> _allFields = new Cache<Type, List<FieldInfo>>(type => type.GetFields(AccessTools.all).ToList());
    private static readonly Cache<(Type, string), FieldInfo> _fieldByName = new Cache<(Type, string), FieldInfo>(pair => pair.Item1.GetField(pair.Item2, AccessTools.all));
    private static readonly Cache<Type, List<MethodInfo>> _methods = new Cache<Type, List<MethodInfo>>(type => type.GetMethods(AccessTools.all).ToList());
    private static readonly Cache<MemberInfo, List<Attribute>> _memberAttrs = new Cache<MemberInfo, List<Attribute>>(member => member.GetCustomAttributes().ToList());


    public static Type GetType(string typeName) => _types.GetCached(typeName);
    public static List<FieldInfo> GetAllFields(Type type) => _allFields.GetCached(type);
    public static FieldInfo GetField(Type type, string name) => _fieldByName.GetCached((type, name));
    public static List<MethodInfo> GetMethods(Type type) => _methods.GetCached(type);
    public static List<Attribute> GetCustomAttributes(MemberInfo member) => _memberAttrs.GetCached(member);
}
