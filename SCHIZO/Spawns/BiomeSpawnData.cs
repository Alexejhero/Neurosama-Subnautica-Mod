using System;
using System.Collections.Generic;
using System.Linq;
using SCHIZO.Helpers;

namespace SCHIZO.Spawns;

partial class BiomeSpawnData
{
    public GameSpecificData Data => IS_BELOWZERO ? belowZero : subnautica;
    public bool Spawn => Data.spawn;
    public BiomeSpawnLocation Location => Data.spawnLocation;
    public IEnumerable<BiomeType> GetBiomes()
    {
        return Location switch
        {
            BiomeSpawnLocation.OpenWater => BiomeHelpers.GetBiomesEndingInAny("_Open", "_Open_CreatureOnly"),
            BiomeSpawnLocation.Surfaces => BiomeHelpers.GetBiomesEndingInAny("Ground", "Wall", "Floor", "Ledge",
                "CaveEntrance", "CavePlants", "SandFlat", "ShellTunnelHuge", "Grass", "Sand", "Mountains", "Beach"),
            BiomeSpawnLocation.Custom => BiomeHelpers.GetBiomesMatching(Data.biomeFilters),
            BiomeSpawnLocation.CopyFromOthers => BiomeHelpers.GetBiomesFor(Data.copyFrom.Cast<TechType>()),
            _ => throw new InvalidOperationException($"Invalid spawn location {Location}")
        };
    }
}
