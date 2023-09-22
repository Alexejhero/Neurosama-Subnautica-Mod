using UnityEngine;

namespace SCHIZO.Unity.Materials
{
    [CreateAssetMenu(fileName = "MaterialRemapConfig", menuName = "SCHIZO/Materials/Material Remap Config")]
    public sealed class MaterialRemapConfig : ScriptableObject
    {
        public Material[] original;
        public MaterialRemapOverride[] remappings;
    }
}
