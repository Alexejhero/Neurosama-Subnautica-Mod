using System;
using System.Collections.Generic;
using System.Linq;

namespace SCHIZO.Helpers;

public static class BiomeHelpers
{
    public static IEnumerable<BiomeType> GetBiomesEndingInAny(params string[] filters)
    {
        if (filters.Length == 0) return [];
        return Enum.GetValues(typeof(BiomeType))
                .Cast<BiomeType>()
                .Where(biome => filters.Any(f => biome.ToString().EndsWith(f)));
    }

    public static IEnumerable<BiomeType> GetBiomesContainingAny(params string[] filters)
    {
        if (filters.Length == 0) return [];
        return Enum.GetValues(typeof(BiomeType))
            .Cast<BiomeType>()
            .Where(biome => filters.Any(f => biome.ToString().Contains(f)));
    }
}
