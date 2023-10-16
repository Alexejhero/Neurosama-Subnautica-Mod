
using System;
using UnityEngine;

namespace SCHIZO.Materials
{
    // [CreateAssetMenu(menuName = "SCHIZO/Materials/Material Remap Override")]
    [Obsolete]
    public sealed class MaterialRemapOverride : ScriptableObject
    {
        public string remapName;
        public Material[] materials;
    }
}
