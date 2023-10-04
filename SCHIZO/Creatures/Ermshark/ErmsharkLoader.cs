using System.Collections.Generic;
using SCHIZO.API.Attributes;
using SCHIZO.API.Creatures;
using SCHIZO.API.Helpers;
using SCHIZO.API.Unity.Creatures;
using SCHIZO.Resources;

namespace SCHIZO.Creatures.Ermshark;

[LoadCreature]
public sealed class ErmsharkLoader : CustomCreatureLoader<CustomCreatureData, ErmsharkPrefab, ErmsharkLoader>
{
    public ErmsharkLoader() : base(ResourceManager.LoadAsset<CustomCreatureData>("Ermshark data"))
    {
        PDAEncyPath = IS_BELOWZERO ? "Lifeforms/Fauna/Carnivores" : "Lifeforms/Fauna/Sharks";
    }

    protected override ErmsharkPrefab CreatePrefab()
    {
        return new ErmsharkPrefab(ModItems.Ermshark, creatureData.regularPrefab);
    }

    protected override IEnumerable<LootDistributionData.BiomeData> GetLootDistributionData()
    {
        foreach (BiomeType biome in BiomeHelpers.GetOpenWaterBiomes())
        {
            yield return new LootDistributionData.BiomeData { biome = biome, count = 1, probability = 0.005f };
        }
    }
}
