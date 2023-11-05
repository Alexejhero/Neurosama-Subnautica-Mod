using System;
using System.Collections.Generic;
using UnityEngine;
using SCHIZO.Registering;
using SCHIZO.Items.Data.Crafting;
using TriInspector;

namespace SCHIZO.Spawns
{
    [CreateAssetMenu(menuName = "SCHIZO/Registering/Coordinated Spawns")]
    public sealed partial class CoordinatedSpawns : ModRegistryItem
    {
        [InfoBox("TODO: use GameAttribute on Item's techtype picker")]
        [ListDrawerSettings(AlwaysExpanded = true)]
        public List<SpawnInfo> spawns;
        [Serializable]
        public sealed partial class SpawnInfo
        {
            [EnumToggleButtons]
            public Game game;
            [Game(nameof(game))]
            public Item item;
            public SpawnLocation[] locations;

            [Serializable]
            public partial struct SpawnLocation
            {
                public Vector3 position;
                public Vector3 rotation;
            }
        }
    }
}
