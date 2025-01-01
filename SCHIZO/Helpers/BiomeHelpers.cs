using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

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

    public static IEnumerable<BiomeType> GetBiomesMatching([StringSyntax("Regex")] params string[] filters)
    {
        if (filters.Length == 0) return [];

        List<Regex> regexes = filters.Select(f => new Regex(f, RegexOptions.Compiled | RegexOptions.Singleline)).ToList();
        return Enum.GetValues(typeof(BiomeType))
            .Cast<BiomeType>()
            .Where(biome => regexes.Any(f => f.IsMatch(biome.ToString())));
    }

    // unfortunately not accessible in main menu (when we register our creatures)
    // hopefully it's not too big of an impact on load time
    internal static readonly LootDistributionData baseGameLootData = LootDistributionData.Load("Balance/EntityDistributions");

    public static IEnumerable<BiomeType> GetBiomesFor(IEnumerable<TechType> techTypes)
    {
        HashSet<BiomeType> set = [];
        foreach (TechType tt in techTypes)
        {
            string classId = CraftData.GetClassIdForTechType(tt);
            if (!baseGameLootData.srcDistribution.TryGetValue(classId, out LootDistributionData.SrcData srcData))
            {
                LOGGER.LogWarning($"Could not get biomes for {tt} - not in loot data!");
                continue;
            }

            List<BiomeType> biomes = srcData.distribution.Select(bd => bd.biome).ToList();
            set.AddRange(biomes);
        }
        LOGGER.LogWarning(string.Join(",", set.Select(b => b.ToString())));
        return set;
    }
}
