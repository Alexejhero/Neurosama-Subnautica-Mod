using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SCHIZO.Interop.Subnautica.Enums;
using SCHIZO.Registering;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Spawns
{
    [CreateAssetMenu(menuName = "SCHIZO/Spawns/Biome Spawn Data")]
    public sealed partial class BiomeSpawnData : ScriptableObject
    {
        [SerializeField, UsedImplicitly, Game(Game.Subnautica)]
        private GameSpecificData subnautica;
        [SerializeField, UsedImplicitly, Game(Game.BelowZero)]
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
            [EnableIf(nameof(spawn)), ShowIf(nameof(spawnLocation), BiomeSpawnLocation.CopyFromOthers)]
            public TechType_All[] copyFrom;
        }
    }
}
