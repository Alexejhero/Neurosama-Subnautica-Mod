using System.Collections.Generic;
using SCHIZO.API.Attributes;
using SCHIZO.API.Creatures;
using SCHIZO.API.Helpers;
using SCHIZO.API.Unity.Creatures;
using SCHIZO.Resources;

namespace SCHIZO.Creatures.Anneel;

[LoadCreature]
public sealed class AnneelLoader : CustomCreatureLoader<CustomCreatureData, AnneelPrefab, AnneelLoader>
{
    public AnneelLoader() : base(ResourceManager.LoadAsset<CustomCreatureData>("Anneel data"))
    {
        PDAEncyPath = "Lifeforms/Fauna/LargeHerbivores";
    }

    protected override AnneelPrefab CreatePrefab()
    {
        return new AnneelPrefab(ModItems.Anneel, creatureData.regularPrefab);
    }

    protected override IEnumerable<LootDistributionData.BiomeData> GetLootDistributionData()
    {
        foreach (BiomeType biome in BiomeHelpers.GetOpenWaterBiomes())
        {
            yield return new LootDistributionData.BiomeData { biome = biome, count = 1, probability = 0.005f };
        }
    }
}
