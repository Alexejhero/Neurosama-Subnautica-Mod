using System;
using System.Collections.Generic;
using System.Linq;

namespace SCHIZO.Helpers;

public static class BiomeHelpers
{
    public static IEnumerable<BiomeType> GetOpenWaterBiomes() => Enum.GetValues(typeof(BiomeType)).Cast<BiomeType>()
        .Where(biome => biome.ToString().EndsWith("CreatureOnly"));
}
