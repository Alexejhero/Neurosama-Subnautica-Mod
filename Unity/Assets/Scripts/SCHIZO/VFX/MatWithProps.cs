using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.VFX
{
    [CreateAssetMenu(menuName = "SCHIZO/Advanced/Effect Material With Properties")]
    public sealed class MatWithProps : ScriptableObject
    {
        [Required]
        public Material material;
        public int vectorPropertyID = -1;
        public Vector4 vectorPropertyValue = Vector4.zero;
        public int colorPropertyID = Shader.PropertyToID("_Color");
        public Color colorPropertyValue = Color.white;
    }
}
