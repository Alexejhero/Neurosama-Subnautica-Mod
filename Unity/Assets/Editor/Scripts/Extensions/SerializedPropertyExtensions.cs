using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.Extensions
{
    public static class SerializedPropertyExtensions
    {
        // code adapted from https://forum.unity.com/threads/get-a-general-object-value-from-serializedproperty.327098/#post-6432620
        private delegate FieldInfo GetFieldInfo(SerializedProperty prop, out Type type);
        private static readonly GetFieldInfo fieldInfoGetter = Setup();

        public static FieldInfo GetFieldInfoAndStaticType(this SerializedProperty prop, out Type type)
        {
            return fieldInfoGetter(prop, out type);
        }

        private static GetFieldInfo Setup()
        {
            Type t = Type.GetType("UnityEditor.ScriptAttributeUtility, UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            MethodInfo mi = t.GetMethod("GetFieldInfoAndStaticTypeFromProperty", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            return (GetFieldInfo) Delegate.CreateDelegate(typeof(GetFieldInfo), mi);
        }

        public static T GetCustomAttribute<T>(this SerializedProperty prop) where T : Attribute
        {
            FieldInfo info = prop.GetFieldInfoAndStaticType(out _);
            return info.GetCustomAttribute<T>();
        }
    }
}
