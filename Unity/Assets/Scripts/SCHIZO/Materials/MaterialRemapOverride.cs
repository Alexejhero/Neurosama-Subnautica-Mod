

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Materials
{
    [CreateAssetMenu(menuName = "SCHIZO/Materials/Material Remap Override")]
    public sealed class MaterialRemapOverride : ScriptableObject
    {
        public string remapName;
        public Material[] materials;
    }
}
