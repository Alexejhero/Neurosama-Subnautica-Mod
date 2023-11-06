using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SCHIZO.Helpers;
using UnityEditor;

namespace Editor.Scripts.Extensions
{
    public static class SerializedPropertyExtensions
    {
        // most (all) of the code is adapted from this thread: https://forum.unity.com/threads/get-a-general-object-value-from-serializedproperty.327098/
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

        public static bool TryGetAttributeInHierarchy<T>(this SerializedProperty prop, out T attribute, out SerializedProperty ancestorWithAttribute) where T : Attribute
        {
            attribute = null;
            ancestorWithAttribute = null;
            foreach (SerializedProperty ancestor in prop.WalkHierarchy())
            {
                attribute = ancestor.GetCustomAttribute<T>();
                if (attribute != null)
                {
                    ancestorWithAttribute = ancestor;
                    return true;
                }
            }
            return false;
        }

        public static SerializedProperty GetParent(this SerializedProperty property)
        {
            int i = property.propertyPath.LastIndexOf('.');
            return i < 0 ? null
                : property.serializedObject.FindProperty(property.propertyPath.Substring(0, i));
        }

        public static IEnumerable<SerializedProperty> WalkHierarchy(this SerializedProperty prop)
        {
            for (SerializedProperty curr = prop; curr != null; curr = curr.GetParent())
                yield return curr;
        }

        public static T GetSerializedValue<T>(this SerializedProperty property)
        {
            // adapted from https://github.com/lordofduct/spacepuppy-unity-framework-4.0/blob/679088a9fca826764de39390b4e08c6feaa06b52/Framework/com.spacepuppy.core/Editor/src/EditorHelper.cs#L278
            string path = property.propertyPath.Replace(".Array.data[", "[");
            object obj = property.serializedObject.targetObject;

            foreach (string elem in path.Split('.'))
            {
                if (elem[elem.Length - 1] == ']')
                {
                    string memberName = elem.Substring(0, elem.LastIndexOf('['));
                    string indexer = elem.Substring(memberName.Length, elem.Length - memberName.Length);
                    int index = int.Parse(indexer.Substring(1, indexer.Length - 2));
                    IList valueList = ReflectionHelpers.GetMemberValue<IList>(obj, memberName);
                    obj = valueList[index];
                }
                else
                {
                   obj = ReflectionHelpers.GetMemberValue<object>(obj, elem);
                }
            }

            return (T)obj;
        }
    }
}
