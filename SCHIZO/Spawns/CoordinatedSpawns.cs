using Nautilus.Handlers;
using NSpawnInfo = Nautilus.Handlers.SpawnInfo;

namespace SCHIZO.Spawns;

partial class CoordinatedSpawns
{
    protected override void Register()
    {
        foreach (SpawnInfo spawnInfo in spawns)
        {
            if (!spawnInfo.game.HasFlag(GAME)) continue;

            foreach (SpawnInfo.SpawnLocation location in spawnInfo.locations)
            {
                NSpawnInfo nSpawnInfo = new((TechType)spawnInfo.item.techType, location.position, location.rotation);
                CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(nSpawnInfo);
            }
        }
    }
}
