using System.Collections.Generic;
using UnityEngine;
using SCHIZO.Registering;
using TriInspector;

namespace SCHIZO.Spawns
{
    [CreateAssetMenu(menuName = "SCHIZO/Spawns/Coordinated Spawns")]
    public sealed partial class CoordinatedSpawns : ModRegistryItem
    {
        [ListDrawerSettings(AlwaysExpanded = true)]
        public List<ItemSpawnInfo> spawns;
    }
}
