using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.VFX
{
    [CreateAssetMenu(menuName = "SCHIZO/Advanced/Effect Material With Properties")]
    public sealed class MatWithProps : ScriptableObject
    {
        [Required]
        public Material material;

        public string floatPropertyName;
        public int floatPropertyID => Shader.PropertyToID(floatPropertyName);
        public float floatPropertyValue = 0f;

        public string vectorPropertyName;
        public int vectorPropertyID => Shader.PropertyToID(vectorPropertyName);
        public Vector4 vectorPropertyValue = Vector4.zero;

        public string colorPropertyName = "_Color";
        public int colorPropertyID => Shader.PropertyToID(colorPropertyName);
        public Color colorPropertyValue = Color.white;
    }
}
