using UnityEditor;
using UnityEngine;
using SCHIZO.VFX;

namespace Editor.Scripts.PropertyDrawers.NonTriInspector.Attributes
{
    [CustomPropertyDrawer(typeof(PreviewImageAttribute), true)]
    public class VFXComponentImagePreview() : PropertyDrawer
    {
        private Texture image;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
        // TODO: this is not ideal 
            image = (Texture) property.objectReferenceValue;
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.alignment = TextAnchor.UpperCenter;
            GUILayout.Box(image, style);
        }
    }
}
