using System;
using SCHIZO.Items.Data.Crafting;
using SCHIZO.Registering;

namespace SCHIZO.Spawns
{
    [Serializable]
    public sealed partial class ItemSpawnInfo : SpawnInfo
    {
        [Game(nameof(game))]
        public Item item;
    }
}
