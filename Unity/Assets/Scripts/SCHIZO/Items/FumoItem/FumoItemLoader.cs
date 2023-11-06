using TriInspector;
using UnityEngine;

namespace SCHIZO.Items.FumoItem
{
    [DeclareBoxGroup("Subnautica")]
    [DeclareBoxGroup("Below Zero")]
    public sealed partial class FumoItemLoader : ItemLoader
    {
        [InfoBox("TODO: remove these when coordinated spawns are in", TriMessageType.Warning)]
        [Group("Subnautica"), LabelText("Spawn on New Game")]
        public bool spawnSN;
        [Group("Subnautica"), LabelText("Spawn Position"), ShowIf(nameof(spawnSN))]
        public Vector3 spawnPositionSN;
        [Group("Subnautica"), LabelText("Spawn Rotation"), ShowIf(nameof(spawnSN))]
        public Vector3 spawnRotationSN;

        [Group("Below Zero"), LabelText("Spawn on New Game")]
        public bool spawnBZ;
        [Group("Below Zero"), LabelText("Spawn Position"), ShowIf(nameof(spawnBZ))]
        public Vector3 spawnPositionBZ;
        [Group("Below Zero"), LabelText("Spawn Rotation"), ShowIf(nameof(spawnBZ))]
        public Vector3 spawnRotationBZ;
    }
}
