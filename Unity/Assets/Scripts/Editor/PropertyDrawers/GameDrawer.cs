using System.Reflection;
using HarmonyLib;
using SCHIZO.Unity;
using UnityEditor;
using UnityEngine;

namespace PropertyDrawers
{
    [CustomPropertyDrawer(typeof(Game))]
    public sealed class GameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, (property.propertyPath + "label").GetHashCode(), label);

            bool isSN = ((Game) property.intValue).HasFlag(Game.Subnautica);
            bool isBZ = ((Game) property.intValue).HasFlag(Game.BelowZero);

            if (!isSN && !isBZ)
            {
                isSN = true;
                isBZ = true;
                property.intValue = (int) (Game.Subnautica | Game.BelowZero);
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            Rect rect = new Rect(position.x, position.y, position.width / 2, position.height);
            using (new EditorGUI.DisabledScope(!isBZ))
            {
                if (ToggleLeft(rect, new GUIContent("Subnautica"), isSN, (property.propertyPath + "sn").GetHashCode()) != isSN)
                {
                    GUI.FocusControl(null);

                    isSN = !isSN;

                    if (isSN)
                    {
                        property.intValue |= (int) Game.Subnautica;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        property.intValue &= ~(int) Game.Subnautica;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }

            rect.x += rect.width;
            using (new EditorGUI.DisabledScope(!isSN))
            {
                if (ToggleLeft(rect, new GUIContent("Below Zero"), isBZ, (property.propertyPath + "bz").GetHashCode()) != isBZ)
                {
                    GUI.FocusControl(null);

                    isBZ = !isBZ;

                    if (isBZ)
                    {
                        property.intValue |= (int) Game.BelowZero;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        property.intValue &= ~(int) Game.BelowZero;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }

            EditorGUI.EndProperty();
        }

        private static readonly MethodInfo _doToggleForward = AccessTools.Method("UnityEditor.EditorGUIInternal:DoToggleForward");

        private static bool ToggleLeft(
            Rect position,
            GUIContent label,
            bool value,
            int controlId)
        {
            Rect position1 = EditorGUI.IndentedRect(position);
            Rect labelPosition = EditorGUI.IndentedRect(position);
            int num = (EditorStyles.toggle.margin.top - EditorStyles.toggle.margin.bottom) / 2;
            labelPosition.xMin += EditorStyles.toggle.padding.left;
            labelPosition.yMin -= num;
            labelPosition.yMax -= num;
            EditorGUI.HandlePrefixLabel(position, labelPosition, label, controlId, EditorStyles.label);
            return (bool) _doToggleForward.Invoke(null, new object[] {position1, controlId, value, GUIContent.none, EditorStyles.toggle});
        }
    }
}
