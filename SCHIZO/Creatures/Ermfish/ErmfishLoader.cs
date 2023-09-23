using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Assets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using SCHIZO.Sounds;
using SCHIZO.Unity.Creatures;
using SCHIZO.Unity.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermfish;

[LoadMethod]
public sealed class ErmfishLoader
{
    // todo: figure out bus path for BZ
    public static readonly SoundPlayer PlayerDeathSounds = new(ResourceManager.LoadAsset<BaseSoundCollection>("Ermfish Player Death"),
        IS_BELOWZERO ? AudioUtils.BusPaths.PDAVoice : "bus:/master/SFX_for_pause/nofilter");

    [LoadMethod]
    private static void Load()
    {
        PickupableCreatureData data = ResourceManager.LoadAsset<PickupableCreatureData>("Ermfish data");

        ErmfishLoader loader = new(data);
        loader.LoadErmfish();
        loader.LoadVariants();
    }

    private readonly PickupableCreatureData _creatureData;
    private readonly CreatureSounds _creatureSounds;

    private ErmfishLoader(PickupableCreatureData data)
    {
        _creatureData = data;
        _creatureSounds = new CreatureSounds(data.soundData);
    }

    private void LoadErmfish()
    {
		ErmfishPrefab ermfish = new(ModItems.Ermfish);
		ermfish.PrefabInfo.WithIcon(_creatureData.regularIcon);
		ermfish.Register();

        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(ermfish, "Lifeforms/Fauna/SmallHerbivores", "Ermfish", _creatureData.databankText.text, 5, _creatureData.databankTexture, _creatureData.unlockSprite);

        KnownTechHandler.SetAnalysisTechEntry(new KnownTech.AnalysisTech
        {
            techType = ModItems.Ermfish,
            unlockTechTypes = new List<TechType>(),
            unlockMessage = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredMessage,
            unlockSound = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredSound,
            unlockPopup = _creatureData.unlockSprite
        });

        List<LootDistributionData.BiomeData> biomes = new();
		foreach (BiomeType biome in BiomeHelpers.GetOpenWaterBiomes())
		{
			biomes.Add(new LootDistributionData.BiomeData { biome = biome, count = 1, probability = 0.025f });
			biomes.Add(new LootDistributionData.BiomeData { biome = biome, count = 5, probability = 0.010f });
		}
		LootDistributionHandler.AddLootDistributionData(ermfish.ClassID, biomes.ToArray());

        PostRegister(ermfish.PrefabInfo);
    }

    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    private void LoadVariants()
    {
        VFXFabricatingData vfxFabricatingData = new("VM/model", -0.255f, 0.67275f, new Vector3(0, 0.22425f), 0.1f, new Vector3(0, -180, 0));

        new CreatureVariant(ModItems.Ermfish, ModItems.CookedErmfish)
        {
            Icon = _creatureData.cookedIcon,
            RecipeData = new RecipeData(new Ingredient(ModItems.Ermfish, 1)),
            EdibleData = new EdibleData(23, 0, true),
            FabricatorPath = CraftTreeHandler.Paths.FabricatorCookedFood,
            TechCategory = Retargeting.TechCategory.CookedFood,
            MaterialRemap = _creatureData.cookedRemap,
            RegisterAsCookedVariant = true,
            VFXFabricatingData = vfxFabricatingData,
            PostRegister = PostRegister,
        }.Register();

        new CreatureVariant(ModItems.Ermfish, ModItems.CuredErmfish)
        {
            Icon = _creatureData.curedIcon,
            RecipeData = new RecipeData(new Ingredient(ModItems.Ermfish, 1), new Ingredient(TechType.Salt, 1)),
            EdibleData = new EdibleData(23, -2, false),
            FabricatorPath = CraftTreeHandler.Paths.FabricatorCuredFood,
            TechCategory = Retargeting.TechCategory.CuredFood,
            MaterialRemap = _creatureData.curedRemap,
            VFXFabricatingData = vfxFabricatingData,
            PostRegister = PostRegister,
        }.Register();
    }

    private void PostRegister(PrefabInfo info)
    {
        CreatureSoundsHandler.RegisterCreatureSounds(info.TechType, _creatureSounds);
        ItemActionHandler.RegisterMiddleClickAction(info.TechType, _ => _creatureSounds.AmbientItemSounds.Play2D(), "pull ahoge", "English");
    }

    public static List<TechType> ErmfishTechTypes => new() { ModItems.Ermfish, ModItems.CookedErmfish, ModItems.CuredErmfish };
}
