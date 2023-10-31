using Nautilus.Handlers;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Items.FumoItem;

partial class FumoItemLoader
{
    public override void Load()
    {
        new FumoItem(itemData.ModItem).Register();
        // TODO: move to a "coordinated spawns" scriptable object?
        if (RetargetHelpers.Pick(spawnSN, spawnBZ))
        {
            Vector3 pos = RetargetHelpers.Pick(spawnPositionSN, spawnPositionBZ);
            Vector3 rot = RetargetHelpers.Pick(spawnRotationSN, spawnRotationBZ);
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(itemData.ModItem, pos, rot));
        }
    }
}
