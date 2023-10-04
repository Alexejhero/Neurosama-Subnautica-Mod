using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.API.Unity.Materials
{
    [CreateAssetMenu(menuName = "SCHIZO/Materials/Material Remap Override")]
    public sealed class MaterialRemapOverride : ScriptableObject
    {
        public string remapName;
        public Material[] materials;
    }
}
