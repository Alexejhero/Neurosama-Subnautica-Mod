using System;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Spawns
{
    [Serializable, InlineEditor]
    public sealed partial class BiomeSpawnRule
    {
        public int count;
        [Range(0, 1)] public float probability;
    }
}
