using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Items.FumoItem
{
    public sealed partial class FumoItemLoader : ItemLoader
    {
        [BoxGroup("Subnautica"), Label("Spawn on New Game")]
        public bool spawnSN;
        [BoxGroup("Subnautica"), Label("Spawn Position"), ShowIf(nameof(spawnSN))]
        public Vector3 spawnPositionSN;
        [BoxGroup("Subnautica"), Label("Spawn Rotation"), ShowIf(nameof(spawnSN))]
        public Vector3 spawnRotationSN;

        [BoxGroup("Below Zero"), Label("Spawn on New Game")]
        public bool spawnBZ;
        [BoxGroup("Below Zero"), Label("Spawn Position"), ShowIf(nameof(spawnBZ))]
        public Vector3 spawnPositionBZ;
        [BoxGroup("Below Zero"), Label("Spawn Rotation"), ShowIf(nameof(spawnBZ))]
        public Vector3 spawnRotationBZ;
    }
}

#if UNITY_EDITOR
namespace SCHIZO.Items.Data
{
    public partial class ItemData
    {
        [ContextMenu("Set Loader/Fumo Item")]
        private void CreateFumoItemLoader() => AssignItemLoader(CreateInstance<FumoItem.FumoItemLoader>());
    }
}
#endif
