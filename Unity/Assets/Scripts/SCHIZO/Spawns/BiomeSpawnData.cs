using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Spawns
{
    [CreateAssetMenu(menuName = "SCHIZO/Spawns/Biome Spawn Data")]
    public sealed class BiomeSpawnData : ScriptableObject
    {
        public BiomeSpawnLocation spawnLocation;
        [TableList] public List<BiomeSpawnRule> rules;
    }
}
