using System;
using System.Collections.Generic;
using System.Linq;

namespace SCHIZO.Helpers;

public static class BiomeHelpers
{
    public static IEnumerable<BiomeType> GetBiomesEndingIn(params string[] filters) => Enum.GetValues(typeof(BiomeType)).Cast<BiomeType>()
        .Where(biome => filters.Any(f => biome.ToString().EndsWith(f)));
}
