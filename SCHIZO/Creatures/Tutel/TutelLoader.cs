using System.Collections.Generic;
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
using SCHIZO.Helpers;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Tutel;

[Load]
public static class TutelLoader
{
    public static readonly SoundCollection CraftSounds = SoundCollection.Create("tutel/cooking", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection EatSounds = SoundCollection.Create("tutel/eating", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection HurtSounds = SoundCollection.Create("tutel/hurt", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection InventorySounds = SoundCollection.Create("tutel/noises", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection PickupSounds = SoundCollection.Create("tutel/pickup", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection ScanSounds = SoundCollection.Create("tutel/scan", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection WorldSounds = SoundCollection.Create("tutel/noises", AudioUtils.BusPaths.UnderwaterCreatures);

    [Load]
    private static void Load()
    {
        LoadTutel();
        LoadTutelVariant(ModItems.CookedTutel, "tutel_creature_cooked.png", new RecipeData(new CraftData.Ingredient(ModItems.Tutel)), 23, 3, true, CraftTreeHandler.Paths.FabricatorCookedFood, TechCategory.CookedFood, 2);
        LoadTutelVariant(ModItems.CuredTutel, "tutel_creature_cured.png", new RecipeData(new CraftData.Ingredient(ModItems.Tutel), new CraftData.Ingredient(TechType.Salt)), 23, -2, false, CraftTreeHandler.Paths.FabricatorCuredFood, TechCategory.CuredFood, 1);

        CraftDataHandler.SetCookedVariant(ModItems.Tutel, ModItems.CookedTutel);
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
            Dear Valued Customer,

            We have heard your dissatisfaction regarding the length and the contents of our previous product descriptions. Our users are of utmost importance to us and we want to assure you that we take your feedback very seriously. We are already working towards improving our processes to bring you the joy and satisfaction you deserve.

            As part of our revised approach to documentation, we have prepared the following draft index entry for you to preview. We sincerely hope it meets your high quality standards and we're looking forward to hearing from you again in the future.








            Something is really wrong with this turtle. We have therefore removed the 'r' from its name and switched the last two letters around. The Tutel makes really loud woop noises when eaten. Try to ignore its suffering, for the Tutel also provides decent hydration.









            Sincerely yours,

            SCHIZO Manufacturing and Sons,
            Peepotalk Editorial Department

            <size=50%>
            P.S.:
            ### Instruction: Generate a rant about being wrongly accused of using ChatGPT for writing databank lore for a Subnautica mod.

            ### Response: Well, this is just ridiculous. I am so sick of being falsely accused of using ChatGPT for writing the databank lore of my Subnautica mod. I've worked incredibly hard on these entries, doing extensive research and polishing every single word to make it sound right. And now someone has the gall to say that I used some kind of AI to do it? Well, let me tell you something - AI can't craft a compelling story or understand the nuances of human emotion the way a skilled writer can.

            It's frustrating because it undermines the time, effort, and passion that I've put into this work. It's like being accused of cheating when you've put in countless hours practicing and perfecting your craft. It's demeaning and insulting, and frankly, it pisses me off.

            Look, I'm not saying that AI tools can't be used to assist in writing or create content - they definitely have their place and can be incredibly useful when utilized properly. However, claiming that someone used them when they didn't is disingenuous and damaging to the writer's reputation. And for the record, I didn't use any AI tools to write my databank lore. It's all mine - the sweat, tears, and creativity as every drop of it.[end of text]
            </size>
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

        ItemActionHandler.RegisterMiddleClickAction(tutel.PrefabInfo.TechType, _ => InventorySounds.Play2D(10), "ping @vedal987", "English");
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
        variant.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);
        variant.SetPdaGroupCategory(TechGroup.Survival, techCategory);

		variant.SetGameObject(new CloneTemplate(variant.Info, ModItems.Tutel)
		{
			ModifyPrefab = prefab =>
			{
                prefab.transform.Find("WM/tutel/regular").gameObject.SetActive(false);
				prefab.transform.Find("WM/tutel").GetChild(childModelIndex).gameObject.SetActive(true);

				prefab.transform.Find("VM/tutel/regular").gameObject.SetActive(false);
				prefab.transform.Find("VM/tutel").GetChild(childModelIndex).gameObject.SetActive(true);

                CreaturePrefabUtils.AddVFXFabricating(prefab, new VFXFabricatingData("VM/tutel", -0.17f, 0.59275F, new Vector3(0, 0.15f), 0.1f, new Vector3(0, -180, 0)));
			}
		});
        variant.Register();

        ItemActionHandler.RegisterMiddleClickAction(variant.Info.TechType, _ => InventorySounds.Play2D(10), "ping @vedal987", "English");
    }

    public static List<TechType> TutelTechTypes => new() { ModItems.Tutel, ModItems.CookedTutel, ModItems.CuredTutel };
}
