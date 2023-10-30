using SCHIZO.Registering;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(Game))]
    public sealed class GameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Validate(property, out bool isSN, out bool isBZ);

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, DrawerUtils.ControlId(property.propertyPath + "label", position), label);

            Rect rect = new Rect(position.x, position.y, position.width / 2, position.height);
            using (new EditorGUI.DisabledScope(!isBZ))
            {
                EditorGUI.BeginChangeCheck();
                isSN = DrawerUtils.ToggleLeft(rect, new GUIContent("Subnautica"), isSN, DrawerUtils.ControlId(property.propertyPath + "sn", rect));
                if (EditorGUI.EndChangeCheck())
                {
                    if (isSN)
                    {
                        property.intValue |= (int) Game.Subnautica;
                    }
                    else
                    {
                        property.intValue &= ~(int) Game.Subnautica;
                    }
                }
            }

            rect.x += rect.width;
            using (new EditorGUI.DisabledScope(!isSN))
            {
                EditorGUI.BeginChangeCheck();
                isBZ = DrawerUtils.ToggleLeft(rect, new GUIContent("Below Zero"), isBZ, DrawerUtils.ControlId(property.propertyPath + "bz", rect));
                if (EditorGUI.EndChangeCheck())
                {
                    if (isBZ)
                    {
                        property.intValue |= (int) Game.BelowZero;
                    }
                    else
                    {
                        property.intValue &= ~(int) Game.BelowZero;
                    }
                }
            }

            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
        }

        private static void Validate(SerializedProperty property, out bool isSN, out bool isBZ)
        {
            isSN = ((Game) property.intValue).HasFlag(Game.Subnautica);
            isBZ = ((Game) property.intValue).HasFlag(Game.BelowZero);

            if (!isSN && !isBZ)
            {
                isSN = true;
                isBZ = true;
                property.intValue = (int) (Game.Subnautica | Game.BelowZero);
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}
