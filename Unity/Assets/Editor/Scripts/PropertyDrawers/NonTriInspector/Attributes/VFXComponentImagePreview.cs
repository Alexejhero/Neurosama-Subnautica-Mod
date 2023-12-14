using UnityEditor;
using UnityEngine;
using SCHIZO.VFX;

namespace Editor.Scripts.PropertyDrawers.NonTriInspector.Attributes
{
    [CustomPropertyDrawer(typeof(PreviewImageAttribute), true)]
    public class VFXComponentImagePreview : PropertyDrawer
    {
        private Texture image;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference) return;
            image = (Texture) property.objectReferenceValue;
            if (!image) return;

            GUIStyle style = new(EditorStyles.label)
            {
                alignment = TextAnchor.UpperCenter,
            };
            GUILayout.Label(image, style, GUILayout.MaxHeight(200f));
        }
    }
}
