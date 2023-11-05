using System;
using System.Collections.Generic;
using UnityEngine;
using SCHIZO.Registering;
using SCHIZO.Items.Data.Crafting;
using NaughtyAttributes;

namespace SCHIZO.Spawns
{
    [CreateAssetMenu(menuName = "SCHIZO/Registering/Coordinated Spawns")]
    public sealed partial class CoordinatedSpawns : ModRegistryItem
    {
        [ReorderableList]
        public List<SpawnInfo> spawns = new List<SpawnInfo>();
        [Serializable]
        public sealed partial class SpawnInfo
        {
            public Item item;
            public SpawnLocation subnautica;
            public SpawnLocation belowZero;

            [Serializable]
            public partial struct SpawnLocation
            {
                public Vector3 position;
                public Vector3 rotation;
            }
        }
    }
}