using System.Collections.Generic;
using UnityEngine;

namespace SCHIZO.Unity
{
    [CreateAssetMenu(fileName = "MaterialRemapConfig", menuName = "SCHIZO/Material Remap Config")]
    public sealed class MaterialRemapConfig : ScriptableObject
    {
        public Material[] original;
        public MaterialRemapOverride[] remappings;
    }
}
