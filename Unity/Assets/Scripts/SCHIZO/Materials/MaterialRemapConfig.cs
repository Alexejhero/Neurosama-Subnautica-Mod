using System;
using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Materials
{
    // [CreateAssetMenu(menuName = "SCHIZO/Materials/Material Remap Config")]
    [Obsolete]
    public sealed class MaterialRemapConfig : ScriptableObject
    {
        public Material[] original;
        [Expandable]
        public MaterialRemapOverride[] remappings;
    }
}
