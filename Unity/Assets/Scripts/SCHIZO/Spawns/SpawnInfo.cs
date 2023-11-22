using System;
using SCHIZO.Items.Data.Crafting;
using SCHIZO.Registering;

namespace SCHIZO.Spawns
{
    [Serializable]
    public sealed partial class SpawnInfo
    {
        public Game game;
        [Game(nameof(game))]
        public Item item;
        public SpawnLocation[] locations;
    }
}
