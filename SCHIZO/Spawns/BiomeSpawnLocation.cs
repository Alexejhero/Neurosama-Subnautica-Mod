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
            BiomeSpawnLocation.OpenWater => BiomeHelpers.GetBiomesEndingIn("CreatureOnly"),
            BiomeSpawnLocation.Surfaces => BiomeHelpers.GetBiomesEndingIn("Wall", "CaveEntrance", "CaveFloor", "CaveWall",
                "SandFlat", "ShellTunnelHuge", "Grass", "Sand", "CaveSand", "CavePlants", "Floor", "Mountains", "RockWall", "Beach", "Ledge"),
            _ => throw new ArgumentOutOfRangeException(nameof(location), location, null)
        };
    }
}
