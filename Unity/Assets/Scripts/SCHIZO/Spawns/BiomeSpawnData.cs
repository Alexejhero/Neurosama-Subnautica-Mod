using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Spawns
{
    [CreateAssetMenu(menuName = "SCHIZO/Spawns/Biome Spawn Data")]
    public sealed partial class BiomeSpawnData : ScriptableObject
    {
        [SerializeField, UsedImplicitly]
        private GameSpecificData subnautica;
        [SerializeField, UsedImplicitly]
        private GameSpecificData belowZero;

        [TableList] public List<BiomeSpawnRule> rules;

        [Serializable]
        public struct GameSpecificData
        {
            public bool spawn;
            [EnableIf(nameof(spawn))]
            public BiomeSpawnLocation spawnLocation;
            [EnableIf(nameof(spawn)), ShowIf(nameof(spawnLocation), BiomeSpawnLocation.Custom)]
            public string[] biomeFilters;
        }
    }
}
