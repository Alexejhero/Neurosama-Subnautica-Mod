using System;
using System.Collections.Generic;
using SCHIZO.Helpers;

namespace SCHIZO.Spawns;

public static class BiomeSpawnLocationExtensions
{
    public static IEnumerable<BiomeType> GetBiomes(this BiomeSpawnLocation location)
    {
        return location switch
        {
            BiomeSpawnLocation.OpenWater => BiomeHelpers.GetBiomesEndingIn("_Open", "_Open_CreatureOnly"),
            BiomeSpawnLocation.Surfaces => BiomeHelpers.GetBiomesEndingIn("Ground", "Wall", "Floor", "Ledge",
                "CaveEntrance", "CavePlants", "SandFlat", "ShellTunnelHuge", "Grass", "Sand", "Mountains", "Beach"),
            _ => throw new ArgumentOutOfRangeException(nameof(location), location, null)
        };
    }
}
