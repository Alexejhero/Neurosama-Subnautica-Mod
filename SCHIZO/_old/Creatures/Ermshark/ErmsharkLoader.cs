using System.Collections.Generic;
using SCHIZO.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using SCHIZO.Unity.Creatures;

namespace SCHIZO.Creatures.Ermshark;

[LoadCreature]
public sealed class ErmsharkLoader : CustomCreatureLoader<CustomCreatureData, ErmsharkPrefab, ErmsharkLoader>
{
    protected override IEnumerable<LootDistributionData.BiomeData> GetLootDistributionData()
    {
        foreach (BiomeType biome in BiomeHelpers.GetOpenWaterBiomes())
        {
            yield return new LootDistributionData.BiomeData { biome = biome, count = 1, probability = 0.005f };
        }
    }
}
