using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.PropertyDrawers
{
    public static class DrawerUtils
    {
        public static int ControlId(string hash, Rect position)
        {
            return GUIUtility.GetControlID(hash.GetHashCode(), FocusType.Keyboard, position);
        }

        private static readonly MethodInfo _doToggleForward = AccessTools.Method("UnityEditor.EditorGUIInternal:DoToggleForward");

        public static bool DoToggleForward(Rect position, int controlId, bool value, GUIContent content, GUIStyle style)
        {
            return (bool) _doToggleForward.Invoke(null, new object[] {position, controlId, value, content, style});
        }

        public static bool ToggleLeft(Rect position, GUIContent label, bool value, int controlId)
        {
            Rect position1 = EditorGUI.IndentedRect(position);
            Rect labelPosition = EditorGUI.IndentedRect(position);
            int num = (EditorStyles.toggle.margin.top - EditorStyles.toggle.margin.bottom) / 2;
            labelPosition.xMin += EditorStyles.toggle.padding.left;
            labelPosition.yMin -= num;
            labelPosition.yMax -= num;
            EditorGUI.HandlePrefixLabel(position, labelPosition, label, controlId, EditorStyles.label);
            return DoToggleForward(position1, controlId, value, GUIContent.none, EditorStyles.toggle);
        }

        private static readonly MethodInfo _doObjectField = AccessTools.GetDeclaredMethods(typeof(EditorGUI)).Single(m => m.Name == "DoObjectField" && m.GetParameters().Length == 9);

        public static Object DoObjectField(Rect position, Rect dropRect, int id, Object obj, System.Type objType, SerializedProperty property, bool allowSceneObjects, GUIStyle style)
        {
            return (Object) _doObjectField.Invoke(null, new object[] {position, dropRect, id, obj, objType, property, null, allowSceneObjects, style});
        }
    }
}
