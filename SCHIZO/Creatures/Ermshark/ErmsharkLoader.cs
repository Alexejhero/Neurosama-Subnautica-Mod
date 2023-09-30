using ECCLibrary;
using Nautilus.Handlers;
using SCHIZO.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using SCHIZO.Sounds;
using SCHIZO.Unity.Creatures;

namespace SCHIZO.Creatures.Ermshark;

[LoadMethod]
public static class ErmsharkLoader
{
    public static GameObject Prefab;

    [LoadMethod]
    private static void Load()
    {
        CustomCreatureData data = ResourceManager.LoadAsset<CustomCreatureData>("Ermshark data");

        ErmsharkPrefab ermshark = new(ModItems.Ermshark, data.prefab);
        ermshark.Register();

        string encyPath = IS_BELOWZERO ? "Lifeforms/Fauna/Carnivores" : "Lifeforms/Fauna/Sharks";

        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(ermshark, encyPath, "Ermshark", data.databankText.text, 5, data.databankTexture, data.unlockSprite);

        List<LootDistributionData.BiomeData> biomes = new();
        foreach (BiomeType biome in BiomeHelpers.GetOpenWaterBiomes())
        {
            biomes.Add(new LootDistributionData.BiomeData { biome = biome, count = 1, probability = 0.005f });
        }
        LootDistributionHandler.AddLootDistributionData(ermshark.PrefabInfo.ClassID, biomes.ToArray());

        CreatureSoundsHandler.RegisterCreatureSounds(ModItems.Ermshark, new CreatureSounds(data.soundData));
    }
}
