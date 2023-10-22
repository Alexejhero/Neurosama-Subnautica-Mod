using UnityEditor;

namespace SCHIZO.Interop.NaughtyAttributes
{
    partial class SerializedPropertyHolder
    {
#if UNITY_EDITOR
        private readonly SerializedProperty _property;

        private SerializedPropertyHolder(SerializedProperty property)
        {
            _property = property;
        }

        public static implicit operator SerializedPropertyHolder(SerializedProperty property) => new SerializedPropertyHolder(property);
        public static implicit operator SerializedProperty(SerializedPropertyHolder holder) => holder._property;
#endif
    }
}
