using System.Collections.Generic;
using SCHIZO.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using SCHIZO.Unity.Creatures;

namespace SCHIZO.Creatures.Anneel;

[LoadCreature]
public sealed class AnneelLoader : CustomCreatureLoader<CustomCreatureData, AnneelPrefab, AnneelLoader>
{
    public AnneelLoader() : base(ResourceManager.LoadAsset<CustomCreatureData>("Anneel data"))
    {
        PDAEncyPath = IS_BELOWZERO ? "Lifeforms/Fauna/Carnivores" : "Lifeforms/Fauna/Sharks";
    }

    protected override AnneelPrefab CreatePrefab()
    {
        return new AnneelPrefab(new ModItem("Anneel", "Anneel"), creatureData.prefab);
    }

    protected override IEnumerable<LootDistributionData.BiomeData> GetLootDistributionData()
    {
        foreach (BiomeType biome in BiomeHelpers.GetOpenWaterBiomes())
        {
            yield return new LootDistributionData.BiomeData { biome = biome, count = 1, probability = 0.005f };
        }
    }
}
