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

namespace SCHIZO.Creatures.Tutel;

[LoadMethod]
public static class TutelLoader
{
    public static readonly CreatureSounds Sounds = new()
    {
        PickupSounds = SoundCollection.Create("tutel/pickup", AudioUtils.BusPaths.PDAVoice),
        // no drop sounds
        CraftSounds = SoundCollection.Create("tutel/cooking", AudioUtils.BusPaths.PDAVoice),
        EatSounds = SoundCollection.Create("tutel/eating", AudioUtils.BusPaths.PDAVoice),
        // no equip/unequip sounds
        ScanSounds = SoundCollection.Create("tutel/scan", AudioUtils.BusPaths.PDAVoice),
        HurtSounds = SoundCollection.Create("tutel/hurt", AudioUtils.BusPaths.PDAVoice),
    };
    public static readonly SoundCollection InventorySounds = SoundCollection.Create("tutel/noises", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection WorldSounds = SoundCollection.Create("tutel/noises", AudioUtils.BusPaths.UnderwaterCreatures);

    [LoadMethod]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    private static void Load()
    {
        PickupableCreatureData data = ResourceManager.AssetBundle.LoadAssetSafe<PickupableCreatureData>("Tutel data");

        LoadTutel(data);
        LoadVariants(data);
    }

    private static void LoadTutel(PickupableCreatureData data)
    {
		TutelPrefab tutel = new(ModItems.Tutel);
		tutel.PrefabInfo.WithIcon(data.regularIcon);
		tutel.Register();

        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(tutel, "Lifeforms/Fauna/SmallHerbivores", "Tutel", data.databankText.text, 5, data.databankTexture, data.unlockSprite);

        KnownTechHandler.SetAnalysisTechEntry(new KnownTech.AnalysisTech
        {
            techType = ModItems.Tutel,
            unlockTechTypes = new List<TechType>(),
            unlockMessage = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredMessage,
            unlockSound = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredSound,
            unlockPopup = data.unlockSprite
        });

        // We need Tutel to spawn low down, not in open waters
        // Otherwise you see it drop like a dead weight to the bottom, and then it starts walking
        // By filtering floors, flats, caves and such, we ensure they spawn on the ground
        string[] filters =
        {
            "Wall",
            "CaveEntrance",
            "CaveFloor",
            "CaveWall",
            "SandFlat",
            "ShellTunnelHuge",
            "Grass",
            "Sand",
            "CaveSand",
            "CavePlants",
            "Floor",
            "Mountains",
            "RockWall",
            "Beach",
            "Ledge"
        };

        List<LootDistributionData.BiomeData> biomes = new();
        foreach (BiomeType biome in BiomeHelpers.GetBiomesEndingIn(filters))
        {
			biomes.Add(new LootDistributionData.BiomeData { biome = biome, count = 1, probability = 0.05f });
			biomes.Add(new LootDistributionData.BiomeData { biome = biome, count = 3, probability = 0.025f });
		}
		LootDistributionHandler.AddLootDistributionData(tutel.ClassID, biomes.ToArray());

        PostRegister(tutel.PrefabInfo);
    }

    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    private static void LoadVariants(PickupableCreatureData data)
    {
        VFXFabricatingData vfxFabricatingData = new("VM/Tutel", -0.17f, 0.59275F, new Vector3(0, 0.15f), 0.1f, new Vector3(0, -180, 0));

        new CreatureVariant(ModItems.Tutel, ModItems.CookedTutel)
        {
            Icon = data.cookedIcon,
            RecipeData = new RecipeData(new Ingredient(ModItems.Tutel, 1)),
            EdibleData = new EdibleData(23, 3, true),
            FabricatorPath = CraftTreeHandler.Paths.FabricatorCookedFood,
            TechCategory = Retargeting.TechCategory.CookedFood,
            MaterialRemap = data.cookedRemap,
            RegisterAsCookedVariant = true,
            VFXFabricatingData = vfxFabricatingData,
            PostRegister = PostRegister,
        }.Register();

        new CreatureVariant(ModItems.Tutel, ModItems.CuredTutel)
        {
            Icon = data.curedIcon,
            RecipeData = new RecipeData(new Ingredient(ModItems.Tutel, 1), new Ingredient(TechType.Salt, 1)),
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
        ItemActionHandler.RegisterMiddleClickAction(info.TechType, _ => InventorySounds.Play2D(10), "ping @vedal987", "English");
    }

    public static List<TechType> TutelTechTypes => new() { ModItems.Tutel, ModItems.CookedTutel, ModItems.CuredTutel };
}
