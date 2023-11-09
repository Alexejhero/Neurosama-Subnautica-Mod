using SCHIZO.HullPlates;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.PropertyDrawers.NonTriInspector.Objects
{
    [CustomPropertyDrawer(typeof(HullPlate))]
    public sealed class HullPlateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HullPlate hullPlate = ((HullPlate) property.objectReferenceValue);

            Color oldColor = GUI.contentColor;
            if (hullPlate && hullPlate.deprecated) GUI.contentColor = Color.gray;

            EditorGUI.PropertyField(position, property, label);

            GUI.contentColor = oldColor;

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
