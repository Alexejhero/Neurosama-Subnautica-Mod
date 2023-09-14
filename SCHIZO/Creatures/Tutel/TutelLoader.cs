using System;
using System.Collections.Generic;
using System.Linq;
using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Attributes;
using SCHIZO.Gadgets;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Tutel;

[Load]
public static class TutelLoader
{
    public static readonly SoundCollection2D CraftSounds = SoundCollection2D.Create("tutel/cooking", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection2D EatSounds = SoundCollection2D.Create("tutel/eating", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection3D HurtSounds = SoundCollection3D.Create("tutel/hurt", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection2D PickupSounds = SoundCollection2D.Create("tutel/pickup", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection2D ScanSounds = SoundCollection2D.Create("tutel/scan", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection3D WorldSounds = SoundCollection3D.Create("tutel/noises", AudioUtils.BusPaths.UnderwaterCreatures); // todo: get rid of fmod erros for loading the same audio file twice

    [Load]
    private static void Load()
    {
        LoadTutel();
        LoadTutelVariant(ModItems.CookedTutel, "tutel_creature_cooked.png", new RecipeData(new CraftData.Ingredient(ModItems.Tutel)), 23, 3, true, CraftTreeHandler.Paths.FabricatorCookedFood, TechCategory.CookedFood, 2);
        LoadTutelVariant(ModItems.CuredTutel, "tutel_creature_cured.png", new RecipeData(new CraftData.Ingredient(ModItems.Tutel), new CraftData.Ingredient(TechType.Salt)), 23, 1, false, CraftTreeHandler.Paths.FabricatorCuredFood, TechCategory.CuredFood, 1);
    }

    private static void LoadTutel()
    {
		TutelPrefab tutel = new(ModItems.Tutel);
		tutel.PrefabInfo.WithIcon(AssetLoader.GetAtlasSprite("tutel_creature.png"));
		tutel.Register();

		Texture2D databankTexture = AssetLoader.GetTexture("tutel-databank.png");
        Sprite unlockSprite = AssetLoader.GetUnitySprite("tutel.png");

        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(tutel, "Lifeforms/Fauna/SmallHerbivores", "Tutel",
			"""
			Something is really wrong with this turtle. We have therefore removed the 'r' from its name and switched the last two letters around. The Tutel makes really loud woop noises when eaten. Try to ignore its suffering, for the Tutel also provides decent hydration.
			""", 5, databankTexture, unlockSprite);

        KnownTechHandler.SetAnalysisTechEntry(new KnownTech.AnalysisTech
        {
            techType = ModItems.Tutel,
            unlockTechTypes = new List<TechType>(),
            unlockMessage = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredMessage,
            unlockSound = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredSound,
            unlockPopup = unlockSprite
        });

        // We need Tutel to spawn low down, not in open waters
        // Otherwise you see it drop like a dead weight to the bottom, and then it starts walking
        // By filtering floors, flats, caves and such, we ensure they spawn on the ground
        string[] filters = {"Wall",
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
         "Ledge"};

        List<LootDistributionData.BiomeData> biomes = new();
        foreach (BiomeType biome in Enum.GetValues(typeof(BiomeType)).Cast<BiomeType>()
            .Where(biome => filters.Any( x => biome.ToString().EndsWith(x) ) ) )
        {
			biomes.Add(new LootDistributionData.BiomeData { biome = biome, count = 1, probability = 0.05f });
			biomes.Add(new LootDistributionData.BiomeData { biome = biome, count = 3, probability = 0.025f });
		}
		LootDistributionHandler.AddLootDistributionData(tutel.ClassID, biomes.ToArray());
    }

    private static void LoadTutelVariant(PrefabInfo info, string iconPath, RecipeData recipe, float foodValue, float waterValue, bool decomposes, string[] craftingTabPath, TechCategory techCategory, int childModelIndex)
    {
	    CustomPrefab variant = new(info);
		variant.Info.WithIcon(AssetLoader.GetAtlasSprite(iconPath));

        CraftingGadget crafting = variant.SetRecipe(recipe);
        crafting.WithFabricatorType(CraftTree.Type.Fabricator);
        crafting.WithStepsToFabricatorTab(craftingTabPath);

        EatableGadget eatable = variant.SetNutritionValues(foodValue, waterValue);
        eatable.WithDecay(decomposes);

        variant.SetUnlock(ModItems.Tutel);
        variant.SetEquipment(EquipmentType.Hand);
        variant.SetPdaGroupCategory(TechGroup.Survival, techCategory);

		variant.SetGameObject(new CloneTemplate(variant.Info, ModItems.Tutel)
		{
			ModifyPrefab = prefab =>
			{
                prefab.transform.Find("WM/tutel/regular").gameObject.SetActive(false);
				prefab.transform.Find("WM/tutel").GetChild(childModelIndex).gameObject.SetActive(true);

				prefab.transform.Find("VM/tutel/regular").gameObject.SetActive(false);
				prefab.transform.Find("VM/tutel").GetChild(childModelIndex).gameObject.SetActive(true);

                CreaturePrefabUtils.AddVFXFabricating(prefab, new VFXFabricatingData("VM/tutel", -0.255f, 0.67275f, new Vector3(0, 0.22425f), 0.1f, new Vector3(0, -180, 0)));
			}
		});
        variant.Register();
    }

    public static List<TechType> TutelTechTypes => new() { ModItems.Tutel, ModItems.CookedTutel, ModItems.CuredTutel };
}
