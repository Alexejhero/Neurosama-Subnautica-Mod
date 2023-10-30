using SCHIZO.Items.Data.Crafting;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(Ingredient))]
    public sealed class IngredientDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty amountProp = property.FindPropertyRelative(nameof(Ingredient.amount));
            int amount = amountProp.intValue;
            if (amount < 1) amountProp.intValue = 1;
            if (amount > 99) amountProp.intValue = 99;

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, DrawerUtils.ControlId(property.propertyPath + "label", position), label);

            Rect itemRect = new Rect(position.x, position.y, position.width - 50, position.height);
            ItemDrawer.DrawItem(property.FindPropertyRelative(nameof(Ingredient.item)), itemRect);

            Rect amountRect = new Rect(position.x + position.width - 45, position.y, 45, position.height);

            EditorGUI.BeginChangeCheck();
            amount = EditorGUI.IntField(amountRect, amount);
            if (EditorGUI.EndChangeCheck())
            {
                amountProp.intValue = amount;
            }

            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
