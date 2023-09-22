using UnityEngine;

namespace SCHIZO.Unity.Materials
{
    [CreateAssetMenu(fileName = "MaterialRemapOverride", menuName = "SCHIZO/Materials/Material Remap Override")]
    public sealed class MaterialRemapOverride : ScriptableObject
    {
        public string remapName;
        public Material[] materials;
    }
}
