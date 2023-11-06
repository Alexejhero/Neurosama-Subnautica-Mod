using System.Collections.Generic;
using UnityEngine;
using SCHIZO.Registering;
using TriInspector;

namespace SCHIZO.Spawns
{
    [CreateAssetMenu(menuName = "SCHIZO/Registering/Coordinated Spawns")]
    public sealed partial class CoordinatedSpawns : ModRegistryItem
    {
        [ListDrawerSettings(AlwaysExpanded = true)]
        public List<SpawnInfo> spawns;
    }
}
