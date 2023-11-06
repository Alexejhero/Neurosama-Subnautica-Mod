using UnityEditor;
using UnityEngine;
using static SCHIZO.Credits.CreditsData;

namespace Editor.Scripts.PropertyDrawers.NonTriInspector.Objects
{
    [CustomPropertyDrawer(typeof(CreditsEntry))]
    public sealed class CreditsEntryDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect nameRect = new(position.x, position.y, position.width / 3, position.height);
            EditorGUI.DelayedTextField(nameRect, property.FindPropertyRelative(nameof(CreditsEntry.name)), GUIContent.none);

            Rect creditsRect = new(position.x + position.width / 3 + 5, position.y, 20, position.height);
            SerializedProperty creditsProperty = property.FindPropertyRelative(nameof(CreditsEntry.credits));

            EditorGUI.BeginChangeCheck();
            CreditsType value = (CreditsType) EditorGUI.EnumFlagsField(creditsRect, (CreditsType) creditsProperty.intValue);
            if (EditorGUI.EndChangeCheck())
            {
                creditsProperty.intValue = (int) value;
            }

            Rect descriptionRect = new(position.x + position.width / 3 + 30, position.y, position.width / 3 * 2 - 30, position.height);
            EditorGUI.LabelField(descriptionRect, value.ToString());

            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
