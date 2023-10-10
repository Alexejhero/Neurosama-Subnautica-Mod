using SCHIZO.Unity.Items;
using UnityEditor;
using UnityEngine;

namespace PropertyDrawers
{
    [CustomPropertyDrawer(typeof(Item))]
    public sealed class ItemDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, DrawerUtils.ControlId(property.propertyPath + "label", position), label);

            DrawItem(property, position);

            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
        }

        public static void DrawItem(SerializedProperty property, Rect position)
        {
            SerializedProperty isCustomProp = property.FindPropertyRelative(nameof(Item.isCustom));
            bool isCustom = isCustomProp.boolValue;

            Rect toggleRect = new Rect(position.x, position.y, EditorStyles.toggle.CalcSize(GUIContent.none).x, position.height);
            EditorGUI.BeginChangeCheck();
            isCustom = DrawerUtils.DoToggleForward(toggleRect, DrawerUtils.ControlId(isCustomProp.propertyPath + "check", toggleRect), isCustom, GUIContent.none, EditorStyles.toggle);
            if (EditorGUI.EndChangeCheck())
            {
                isCustomProp.boolValue = isCustom;
            }

            position.xMin += EditorStyles.toggle.padding.left;
            if (isCustom)
            {
                SerializedProperty prop = property.FindPropertyRelative(nameof(Item.itemData));
                DrawerUtils.DoObjectField(position, position, DrawerUtils.ControlId(prop.propertyPath + "itemdata", position), prop.objectReferenceValue, typeof(ItemData), prop, false, EditorStyles.objectField);
            }
            else
            {
                SerializedProperty prop = property.FindPropertyRelative(nameof(Item.techType));
                TechType_AllDrawer.DrawDropdownButton(prop, DrawerUtils.ControlId(prop.propertyPath + "techtype", position), position);
            }
        }
    }
}
