using System;
using SCHIZO.Registering;

namespace SCHIZO.Spawns
{
    [Serializable]
    public partial class SpawnInfo
    {
        public Game game;
        public SpawnLocation[] locations;
    }
}
