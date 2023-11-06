using System;
using SCHIZO.Items.Data.Crafting;
using SCHIZO.Registering;
using UnityEngine;

namespace SCHIZO.Spawns
{
    [Serializable]
    public sealed partial class SpawnInfo
    {
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
