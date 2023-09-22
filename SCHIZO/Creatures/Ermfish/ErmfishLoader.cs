using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Assets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Attributes;
using SCHIZO.Extensions;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using SCHIZO.Sounds;
using SCHIZO.Unity.Creatures;
using UnityEngine;

namespace SCHIZO.Creatures.Ermfish;

[LoadMethod]
public static class ErmfishLoader
{
    public static readonly CreatureSounds Sounds = new()
    {
        PickupSounds = SoundCollection.Create("ermfish/pickup", AudioUtils.BusPaths.PDAVoice),
        DropSounds = SoundCollection.Create("ermfish/release", AudioUtils.BusPaths.PDAVoice),
        CraftSounds = SoundCollection.Create("ermfish/cooking", AudioUtils.BusPaths.PDAVoice),
        EatSounds = SoundCollection.Create("ermfish/eating", AudioUtils.BusPaths.PDAVoice),
        EquipSounds = SoundCollection.Create("ermfish/equipping", AudioUtils.BusPaths.PDAVoice),
        UnequipSounds = SoundCollection.Create("ermfish/unequipping", AudioUtils.BusPaths.PDAVoice),
        ScanSounds = SoundCollection.Create("ermfish/scan", AudioUtils.BusPaths.PDAVoice),
        HurtSounds = SoundCollection.Create("ermfish/hurt", AudioUtils.BusPaths.PDAVoice)
    };
    public static readonly SoundCollection InventorySounds = SoundCollection.Create("ermfish/noises", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection PlayerDeathSounds = SoundCollection.Create("ermfish/player_death", IS_BELOWZERO ? AudioUtils.BusPaths.PDAVoice : "bus:/master/SFX_for_pause/nofilter");
    public static readonly SoundCollection WorldSounds = SoundCollection.Create("ermfish/noises", AudioUtils.BusPaths.UnderwaterCreatures);

    [LoadMethod]
    private static void Load()
    {
        PickupableCreatureData data = ResourceManager.AssetBundle.LoadAssetSafe<PickupableCreatureData>("Ermfish data");

        LoadErmfish(data);
        LoadVariants(data);
    }

    private static void LoadErmfish(PickupableCreatureData data)
    {
		ErmfishPrefab ermfish = new(ModItems.Ermfish);
		ermfish.PrefabInfo.WithIcon(data.regularIcon);
		ermfish.Register();

        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(ermfish, "Lifeforms/Fauna/SmallHerbivores", "Ermfish", data.databankText.text, 5, data.databankTexture, data.unlockSprite);

        KnownTechHandler.SetAnalysisTechEntry(new KnownTech.AnalysisTech
        {
            techType = ModItems.Ermfish,
            unlockTechTypes = new List<TechType>(),
            unlockMessage = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredMessage,
            unlockSound = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredSound,
            unlockPopup = data.unlockSprite
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
    private static void LoadVariants(PickupableCreatureData data)
    {
        VFXFabricatingData vfxFabricatingData = new("VM/model", -0.255f, 0.67275f, new Vector3(0, 0.22425f), 0.1f, new Vector3(0, -180, 0));

        new CreatureVariant(ModItems.Ermfish, ModItems.CookedErmfish)
        {
            Icon = data.cookedIcon,
            RecipeData = new RecipeData(new Ingredient(ModItems.Ermfish, 1)),
            EdibleData = new EdibleData(23, 0, true),
            FabricatorPath = CraftTreeHandler.Paths.FabricatorCookedFood,
            TechCategory = Retargeting.TechCategory.CookedFood,
            MaterialRemap = data.cookedRemap,
            RegisterAsCookedVariant = true,
            VFXFabricatingData = vfxFabricatingData,
            PostRegister = PostRegister,
        }.Register();

        new CreatureVariant(ModItems.Ermfish, ModItems.CuredErmfish)
        {
            Icon = data.curedIcon,
            RecipeData = new RecipeData(new Ingredient(ModItems.Ermfish, 1), new Ingredient(TechType.Salt, 1)),
            EdibleData = new EdibleData(23, -2, false),
            FabricatorPath = CraftTreeHandler.Paths.FabricatorCuredFood,
            TechCategory = Retargeting.TechCategory.CuredFood,
            MaterialRemap = data.curedRemap,
            VFXFabricatingData = vfxFabricatingData,
            PostRegister = PostRegister,
        }.Register();
    }

    private static void PostRegister(PrefabInfo info)
    {
        CreatureSoundsHandler.RegisterCreatureSounds(info.TechType, Sounds);
        ItemActionHandler.RegisterMiddleClickAction(info.TechType, _ => InventorySounds.Play2D(), "pull ahoge", "English");
    }

    public static List<TechType> ErmfishTechTypes => new() { ModItems.Ermfish, ModItems.CookedErmfish, ModItems.CuredErmfish };
}
