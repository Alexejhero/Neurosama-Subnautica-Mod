using UnityEngine;

namespace SCHIZO.Unity
{
    [CreateAssetMenu(fileName = "MaterialRemapOverride", menuName = "SCHIZO/Material Remap Override")]
    public sealed class MaterialRemapOverride : ScriptableObject
    {
        public string remapName;
        public Material[] materials;
    }
}
