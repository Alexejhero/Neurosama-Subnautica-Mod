using Nautilus.Handlers;
using SCHIZO.Helpers;
using NSpawnInfo = Nautilus.Handlers.SpawnInfo;

namespace SCHIZO.Spawns;

partial class CoordinatedSpawns
{
    protected override void Register()
    {
        foreach (SpawnInfo spawnInfo in spawns)
        {
            //if (!spawnInfo.game.HasFlag(GAME)) continue;

            //foreach (SpawnInfo.SpawnLocation location in spawnInfo.spawnLocations)
            SpawnInfo.SpawnLocation location = RetargetHelpers.Pick(spawnInfo.subnautica, spawnInfo.belowZero);
            NSpawnInfo nSpawnInfo = new((TechType)spawnInfo.item.techType, location.position, location.rotation);
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(nSpawnInfo);
        }
    }
}
